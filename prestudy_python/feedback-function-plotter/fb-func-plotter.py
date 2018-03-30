import numpy as np
import matplotlib.pyplot as plt
import collections as col


DIST_MIN     = 20
DIST_MAX     = 300
ACT_FREQ_MIN = 1
ACT_FREQ_MAX = 7

DIST_DIFF = DIST_MAX - DIST_MIN
ACT_FREQ_RATIO = ACT_FREQ_MIN / ACT_FREQ_MAX

# Slope of the linear feedback function
LIN_SLOPE = (ACT_FREQ_MIN - ACT_FREQ_MAX) / DIST_DIFF
# y-offset of the linear feedback function
LIN_OFF   = ACT_FREQ_MIN - DIST_MAX * LIN_SLOPE


# Plot fontsizes
TICKS_LEGEND = 24
AXIS_LABELS  = 26


def linear_func(_x):
    return LIN_SLOPE * _x + LIN_OFF


def feedback_func(x):
    return ACT_FREQ_MAX * ACT_FREQ_RATIO ** ((x - DIST_MIN) / DIST_DIFF)


def main():
    _xs = np.linspace(20, 300, 281)
    _ys_exp = feedback_func(_xs)
    _ys_lin = linear_func(_xs)
    
    plt.plot(_xs, _ys_lin, label='Lin.')
    plt.plot(_xs, _ys_exp, label='Exp.')
    plt.xlabel("Distance (cm)", fontsize=AXIS_LABELS)
    plt.ylabel("Activation Frequency (Hz)", fontsize=AXIS_LABELS)
    plt.xlim((0, 320))
    plt.ylim((0, 8))
    plt.xticks(np.arange(0, 321, 20), fontsize=TICKS_LEGEND)
    plt.yticks(fontsize=TICKS_LEGEND)
    plt.grid(True)
    plt.legend(fontsize=TICKS_LEGEND)
    plt.show()


if __name__ == '__main__':
    main()