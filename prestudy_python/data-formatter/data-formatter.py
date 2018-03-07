#! /usr/bin/env python3


import glob;
import csv;


path = "../data/*"
header = ["Participant_ID", "Referencepoint_Distance", "Obstacle_Distance", "Correct_Answer"]
referencepoints = {"0": "55", "1": "125", "2": "195", "3": "265"}

def format_file(file):
    print(file.read())


def read_files():
    for file in glob.glob(path):
        with open(file) as in_file:
            reader = csv.reader(in_file)

            name = file[1:]  # write into the data directory inside this directory
            with open(name, 'w') as out_file:

                # write header
                writer = csv.writer(out_file)
                writer.writerow(header)

                # write row from input file, but with midpoint values instead of numbers
                for row in reader:
                    new_row = [row[0], referencepoints[row[1]], row[2], row[3]]
                    writer.writerow(new_row)

def main():
    read_files()


if __name__ == "__main__":
    main()
