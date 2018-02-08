"""Script to plot our prestudy data."""

import csv
import collections
import matplotlib.pyplot as plt


PATH = './data/all.csv'
REF_POINTS = [55, 125, 195, 265]
METRIC_MAP = {
    'participant': 0,
    'refpoint': 1,
    'distance': 2
}


def str2bool(_str):
    """Casts string to bool."""
    return True if _str == 'True' else False


def load_csv(path=PATH):
    """Loads and parses the csv data."""
    with open(path) as csvfile:
        reader = csv.reader(csvfile)
        for row in reader:
            yield [float(row[0]),
                   REF_POINTS[int(row[1])],
                   float(row[2]),
                   str2bool(row[3])]


def get_column(data, column_index):
    """Returns generator of specific column in 2-d data."""
    for row in data:
        yield row[column_index]


def _applies_by_metric(row, _by):
    """Checks whether data row applies for given metric(s)."""
    for metric, value in _by.items():
        if isinstance(value, collections.Iterable):
            # Search for value that applies
            for _v in value:
                if _v == row[METRIC_MAP[metric]]:
                    # If found, continue with outer loop
                    break
            else:
                # If not found, this metric and therefore row does not apply
                return False
        elif value != row[METRIC_MAP[metric]]:
            # If value not iterable check directly if metric applies.
            # If it does not, whole row also does not
            return False
    # At this point, every metric and therefore the whole row applies
    return True


def _get_relerr(data, _by):
    """Get relative error by given metric(s)."""
    negatives = 0
    total = 0
    for row in data:
        if _applies_by_metric(row, _by):
            total += 1
            if row[3] is False:
                negatives += 1
    return negatives / total


def get_relerr_by_distance(data, distance):
    """Get relative error based on moved distance.
    distance: int, Iterable(int)
    """
    return _get_relerr(data, {'distance': distance})


def get_relerr_by_refpoint(data, refpoint):
    """Get relative error based on reference point.
    refpoint: int, Iterable(int)
    """
    return _get_relerr(data, {'refpoint': refpoint})


def get_relerr_by_participant(data, participant):
    """Get relative error based on participant ID.
    participant: int, Iterable(int)
    """
    return _get_relerr(data, {'participant': participant})


def _plot_relerr(_x, _y, xlab, barwidth):
    """Generic bar-plot function."""
    _, _ax = plt.subplots()
    _ax.bar(x=_x, height=_y, width=(4/5 * barwidth), color='#555E67')
    _ax.set_axisbelow(True)
    _ax.grid(linestyle='dashed')
    plt.ylabel("Relative Error")
    plt.xlabel(xlab)
    plt.show()


def plot_relerr_by_distances(data):
    """Sets up a bar-subplot for the relative error by moved distance."""
    step_size = 5
    _x = list(range(-30, -1, step_size)) + list(range(5, 31, step_size))
    _y = [get_relerr_by_distance(data, x) for x in _x]
    _plot_relerr(_x, _y, "Distance from Reference Point", step_size)


def plot_relerr_by_refpoints(data):
    """Sets up a bar-subplot for the relative error by reference point."""
    step_size = 70
    _x = list(range(55, 300, step_size))
    _y = [get_relerr_by_refpoint(data, x) for x in _x]
    _plot_relerr(_x, _y, "Sensor to Reference Point Distance", step_size)


def plot_relerr_by_participants(data):
    """Sets up a bar-subplot for the relative error by participant."""
    step_size = 1
    _x = list(range(1, 7, step_size))
    _y = [get_relerr_by_participant(data, x) for x in _x]
    _plot_relerr(_x, _y, "Participant ID", step_size)


def get_weighted_distance_avg(data):
    """Get the psychometric value for the distance difference of reference
    points."""
    weighted_sum = 0
    for row in data:
        weighted_sum += abs(row[2]) * int(not row[3])
    return weighted_sum / len(data)


def main():
    """Main function."""
    data = list(load_csv())
    print(get_weighted_distance_avg(data))
    plot_relerr_by_distances(data)
    plot_relerr_by_refpoints(data)
    plot_relerr_by_participants(data)


if __name__ == '__main__':
    main()
