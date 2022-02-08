
import pandas as pd
import os
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import matplotlib as mpl
import numpy as np
import seaborn as sns
import matplotlib.patheffects as path_effects
from scipy import stats
from matplotlib import pyplot
from statsmodels.graphics.gofplots import qqplot
# QQ Plot
from numpy.random import seed
from numpy.random import randn
import helpers as hlp


exact_df = hlp.getExactData()
# Exact data


if not os.path.exists("exact_plots/"):
    os.makedirs("exact_plots/")

def plot_pcs(exact_df):
    for x in range(len(hlp.cinemas)):
        f1, axs = plt.subplots(figsize=hlp.get_figsize(wf=3.7, hf=0.26))
        cinema = hlp.cinemas[x]
   
        data = exact_df.loc[exact_df['Cinema_Room'] == cinema]


        axs.set_title(f"{x + 1}: Cinema = {cinema} (Seats = {int(data['Capacity'].mean())})")
            
        if (cinema == "Tillburg_4"):
            axs.set_title(f"{x + 1}: Cinema = Tilburg (Seats = {int(data['Capacity'].mean())})")

        data.sort_values('display_name')

        palette = sns.color_palette("Blues", n_colors=4)
        palette.reverse()
        #axs.legend(title = None)
        hst = sns.histplot(
            data,
            x='display_name',
            weights='PercentagePeopleSeated',
            hue='occupancy_label',
            multiple='stack',
            palette=palette,
            edgecolor='white',
            hue_order = ['very high occupancy','high occupancy', 'medium occupancy', 'low occupancy'],
            shrink=0.8
        )
        hst.set_xticks(range(10 + 1))
        hst.set_xticklabels (['SEAT_GREEDY_LF', 'SEAT_GREEDY_SF', 'SEAT_GREEDY_RAND', 'SEAT_GREEDY_IS_LF', 'SEAT_GREEDY_IS_SF', 'SEAT_GREEDY_IS_RAND',  'SEAT_GREEDY_MADS', 'SEAT_ILP_IS', 'SEAT_ILP_MADS', 'SEAT_ILP',  ""])

        axs.legend(['low occupancy', 'medium occupancy', 'high occupancy', 'very high occupancy'], title="Occupancy Type")

        axs.set(xlabel='Solver Type', ylabel='PPS')
      
        f1.savefig(f'exact_plots/pcs_{cinema}.pdf', bbox_inches='tight', pad_inches = 0.1)


def plot_time(exact_df):
    f2, axs2 = plt.subplots(figsize=hlp.get_figsize(wf=3.2, hf=0.3))
    axs2.set(xlabel='Cinema Size', ylabel='Average Solving Time (Seconds)')
    exact_df = exact_df.loc[exact_df['SolverType'] == "Greedy_SF"]
    sns.lineplot(data=exact_df, x="Size", y="SolvingTime", hue="display_name", ax=axs2, ci=False, marker='o')
    plt.legend(title='Solver Type')
    f2.savefig('exact_plots/solving_times.pdf', bbox_inches='tight', pad_inches = 0.1,)

plot_pcs(exact_df)