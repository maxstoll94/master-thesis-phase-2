import pandas as pd
import os
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import matplotlib as mpl
import numpy as np
import seaborn as sns
import matplotlib.patheffects as path_effects
from scipy import stats

import helpers as hlp

if not os.path.exists("greedy_repeat_plots/"):
    os.makedirs("greedy_repeat_plots/")

repeat_random_df = hlp.getRandomRepeatData()
means_greedy_mis = []
means_greedy = []

def maxDiff(arr, arr_size):
    max_diff = arr[1] - arr[0]
     
    for i in range( 0, arr_size ):
        for j in range( i+1, arr_size ):
            if(arr[j] - arr[i] > max_diff):
                max_diff = arr[j] - arr[i]
     
    return max_diff

df1 = repeat_random_df.loc[(repeat_random_df['SolverType'] == "Greedy_MIS_Random")]
for r in range(hlp.number_of_repeats):
        df2 = df1.loc[(df1['RepeatId'] == r)]
        means_greedy_mis.append(df2['PercentageSeated'].mean())

df1 = repeat_random_df.loc[(repeat_random_df['SolverType'] == "Greedy_Random")]
for r in range(hlp.number_of_repeats):
        df2 = df1.loc[(df1['RepeatId'] == r)]
        means_greedy.append(df2['PercentageSeated'].mean())

print(means_greedy_mis)
print("Maximum difference is", maxDiff(means_greedy_mis, len(means_greedy_mis)))
print("Maximum difference is", maxDiff(means_greedy, len(means_greedy)))