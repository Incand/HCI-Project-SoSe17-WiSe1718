"""
public float GaussLike(float x)
{
    float exp = 1.0f / GAUSS_STD_DEV * (_angle - x);
    return _getFalloff() * Mathf.Exp(-0.5f * (exp * exp));
}
"""

import numpy as np
import matplotlib.pyplot as plt


RANGE         = 90
N_ACTUATORS   = 4

GAUSS_STD_DEV = RANGE / N_ACTUATORS
RANGE_TUP     = (-RANGE, RANGE+1)

# Plot fontsizes
TICKS_LEGEND = 24
AXIS_LABELS  = 26


def get_gauss_like(lifetime, angle, _x):
    exp = 1 / GAUSS_STD_DEV * (angle - _x)
    return lifetime * np.exp(-0.5 * exp * exp)


def main():
    _xs = np.linspace(*RANGE_TUP, 1000)
    _ys = get_gauss_like(0.5, -45, _xs)
    plt.plot(_xs, _ys)
    plt.xlabel("Angle (Degree)", fontsize=AXIS_LABELS)
    plt.ylabel("Amplitude", fontsize=AXIS_LABELS)
    plt.xlim((-RANGE, RANGE))
    plt.ylim((0, 1.1))
    plt.xticks(np.arange(*RANGE_TUP, 15), fontsize=TICKS_LEGEND)
    plt.yticks(fontsize=TICKS_LEGEND)
    plt.grid(True)
    plt.show()


if __name__ == '__main__':
    main()