import argparse
import sys
import os
import os.path
import csv
import random


description = 'Tool to create data folder structures and csv\'s for\
              the prestudy'


def prompt_binary(question, pos, neg):
    prompt = ' [' + pos + '/' + neg + ']'
    while True:
        sys.stdout.write(question + prompt)
        choice = input().lower()
        if choice == pos:
            return True
        elif choice == neg:
            return False
        else:
            print("Invalid choice")


def get_args():
    parser = argparse.ArgumentParser(description=description)
    parser.add_argument('p_num', type=int, help='Participant number')
    parser.add_argument('mp_num', type=int, help='Midpoint number')
    constconv = parser.add_mutually_exclusive_group()
    constconv.add_argument('-a', '--constant', action='store_const',
                           dest='distrib', const='constant',
                           help='Use constant distribution (default)')
    constconv.add_argument('-b', '--converge', action='store_const',
                           dest='distrib', const='converge',
                           help='Use converge distribution')
    parser.add_argument('-o', '--output-file', dest='out_path', type=str,
                        help='Relative output path (default: ./data)')
    parser.add_argument('-t', '--trials', dest='trials', type=int,
                        help='Number of trials per participant per midpoint '
                        '(default: 25)')
    parser.set_defaults(distrib='constant', out_path='./data', trials=25)
    return parser.parse_args()


def handle_aborov(path):
    if os.path.exists(path):
        aborov = prompt_binary('Csv for this participant and midpoint already '
                               'exists.',
                               'a', 'o')
        if aborov:
            exit()
        else:
            os.remove(path)


get_random_distance = None


def set_random_distance_function(distrib):
    global get_random_distance
    if distrib == 'constant':
        get_random_distance = _get_random_dist_constant
    else:
        get_random_distance = _get_random_dist_converge


def _get_random_dist_constant(mp_num):
    return round(random.uniform(-35, 35))


def _get_random_dist_converge(mp_num):
    return 0


def write_csv(p_num, mp_num, trials):
    sp_num, smp_num = str(p_num), str(mp_num)
    if not os.path.exists('./data'):
        os.mkdir('./data')
    if not os.path.exists('./data/' + sp_num):
        os.mkdir('./data/' + sp_num)

    with open('./data/' + sp_num + '_' + smp_num + '.csv', 'w') as f:
        writer = csv.writer(f)
        for i in range(1, trials+1):
            print('Conduction trial no. ' + str(i) + '...')
            dist = get_random_distance(mp_num)
            print('Set distance to ' + str(dist) + 'cm from reference '
                  'object.')
            res = prompt_binary('Did the participant answer correctly?',
                                'y', 'n')
            row = [p_num, mp_num, dist, res]
            writer.writerow(row)
    print('Trails done!')


def main():
    args = get_args()
    set_random_distance_function(args.distrib)
    path = args.out_path + '/' + str(args.p_num) \
        + '/' + str(args.mp_num)
    handle_aborov(path)
    write_csv(args.p_num, args.mp_num, args.trials)


if __name__ == '__main__':
    main()
