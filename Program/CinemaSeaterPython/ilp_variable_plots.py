
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


def draw_ilp_variables_bar(solve_df):
    print("Drawing ilp variables bar.")

    fig, ax1 = plt.subplots(figsize=hlp.get_figsize(wf=3.2, hf=0.3))

    bar1 = sns.barplot(data=solve_df, x="Size", y="VariableCount", hue="display_name", palette="ocean", ax=ax1, ci=None)
    bar1.set( ylabel="Binary Variable Count", xlabel="Cinema Size")

    for p in bar1.patches:
        bar1.annotate(round(p.get_height()), 
                        (p.get_x() + p.get_width() / 2., p.get_height()), 
                        ha = 'center', va = 'center', 
                        xytext = (0, 5), 
                        textcoords = 'offset points')

    ax1.legend(title="Solver Type")
    fig.savefig('variable_plots/ilp_variables_bar.pdf', bbox_inches='tight', pad_inches = 0.1)

def draw_ilp_variables_time(solve_df):
    print("Drawing ilp variables time.")

    fig, ax1 = plt.subplots(figsize=hlp.get_figsize(wf=3.2, hf=0.3))

    sns.lineplot(data=solve_df, x="VariableCount", y="SolvingTime", hue="display_name", palette="ocean", ax=ax1, marker='o', style="display_name", ci=None).set( ylabel="Average Solving Time (Seconds)", xlabel="Binary Variable Count")
    ax1.legend(title="Solver Type")
    fig.savefig('variable_plots/ilp_variables_time.pdf', bbox_inches='tight', pad_inches = 0.1)


if not os.path.exists("variable_plots/"):
    os.makedirs("variable_plots/")

s_df = hlp.getRandomSolverDf()
v_df = hlp.getVariableData()

s_ilp = s_df.loc[s_df['SolverType'] == "ILP"].reset_index(drop=True)
v_ilp =  v_df.loc[v_df['SolverType'] == "ILP"].reset_index(drop=True)
v_ilp["SolvingTime"] = s_ilp["SolvingTime"]

s_ilp_is = s_df.loc[s_df['SolverType'] == "ILP_MIS"].reset_index(drop=True)
v_ilp_is =  v_df.loc[v_df['SolverType'] == "ILP_MIS"].reset_index(drop=True)
v_ilp_is["SolvingTime"] = s_ilp_is["SolvingTime"]

s_ilp_mads = s_df.loc[s_df['SolverType'] == "MADS_ILP"].reset_index(drop=True)
v_ilp_mads =  v_df.loc[v_df['SolverType'] == "MADS_ILP"].reset_index(drop=True)
v_ilp_mads["SolvingTime"] = s_ilp_mads["SolvingTime"]

draw_ilp_variables_bar(pd.concat([ v_ilp_is, v_ilp, v_ilp_mads], ignore_index=True))
draw_ilp_variables_time(pd.concat([ v_ilp, v_ilp_mads], ignore_index=True))