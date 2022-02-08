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

plt.rcParams['legend.title_fontsize'] = 'small'
plt.rcParams['legend.fontsize'] = 'small'

def update_solver_type(value, constraint):
    return f"{constraint}_{value}"


def draw_diagnal_vs_without_diagnal(df):
    print("Drawing diagnal vs without diagnal.")

    fig, ax1 = plt.subplots()
    bar = sns.barplot(data = df, x='Distance_Constraints', y='PercentageSeated', palette='Oranges', ci = None, ax=ax1)
    bar.set(ylabel="Average PCS", xlabel="Constraint Type")

    for p in bar.patches:
        bar.annotate(format(p.get_height(), '.2f'), 
                       (p.get_x() + p.get_width() / 2., p.get_height()), 
                       ha = 'center', va = 'center', 
                       xytext = (0, 5), 
                       textcoords = 'offset points')

    fig.savefig('exclude_diagnal_plots/pcs.pdf', bbox_inches='tight', pad_inches = 0.1)

    fig, ax1 = plt.subplots()

    bar = sns.barplot(data = df, x='Distance_Constraints', y='PercentagePeopleSeated', palette='Oranges', ci = None, ax=ax1)
    bar.set(ylabel="Average PPS", xlabel="Constraint Type")

    for p in bar.patches:
        print(p)
        bar.annotate(format(p.get_height(), '.2f'), 
                       (p.get_x() + p.get_width() / 2., p.get_height()), 
                       ha = 'center', va = 'center', 
                       xytext = (0, 5), 
                       textcoords = 'offset points')
    fig.savefig('exclude_diagnal_plots/pps.pdf', bbox_inches='tight', pad_inches = 0.1)

    fig, ax1 = plt.subplots()
    df = df[df['SolverType'].str.contains('Greedy')]
    bar = sns.barplot(data = df, x='Distance_Constraints', y='SolvingTime', palette='Oranges', ci = None, ax=ax1)
    bar.set(ylabel="Average Solving Time (Seconds)", xlabel="Constraint Type")

    for p in bar.patches:
        print(p)
        bar.annotate(format(p.get_height(), '.2f'), 
                       (p.get_x() + p.get_width() / 2., p.get_height()), 
                       ha = 'center', va = 'center', 
                       xytext = (0, 5), 
                       textcoords = 'offset points')
    fig.savefig('exclude_diagnal_plots/solvingTime.pdf', bbox_inches='tight', pad_inches = 0.1)

def draw_pcs_boxplots(solve_df):
    print("Drawing box plots for PCS.")
    withMIS = solve_df.loc[solve_df['MISType'] != 'None'].sort_values('SolverType')
    withoutMIS = solve_df.loc[(solve_df['MISType'] == 'None')].sort_values('SolverType')

    withMISg = withMIS.groupby('SolverType')
    withoutMISg = withoutMIS.groupby('SolverType')

    f1, (ax1) = plt.subplots(figsize=hlp.get_figsize(wf=2.1, hf=0.65))
    ax1.set(ylim=(-0.1, 1.1))

    b1 = sns.boxplot(x="SolverType", y="PercentageSeated", hue="MISType",
                    data=withMIS, ax=ax1, palette="summer", showmeans=True, meanprops={"marker":"o", "markerfacecolor":"white", "markeredgecolor":"black", "markersize":"10"})
    b1.set_xticklabels(['%s\n($n$=%d)'%(k, len(v)) for k, v in withMISg])
    ax1.set(xlabel='Solver Type', ylabel='PCS')

    f2, (ax3) = plt.subplots(figsize=hlp.get_figsize(wf=2.1, hf=0.65))
    ax3.set(ylim=(-0.1, 1.1))

    b2 = sns.boxplot(x="SolverType", y="PercentageSeated",
                    data=withoutMIS, ax=ax3, hue="size_label", palette="summer")
    b2.set_xticklabels(['%s\n($n$=%d)'%(k, len(v)) for k, v in withoutMISg])
    ax3.set(xlabel='Solver Type', ylabel='PCS')

    hlp.add_median_labels(ax1, '.3f')
    hlp.add_median_labels(ax3, '.3f')
    
    f1.savefig('exclude_diagnal_plots/with_MIS.pdf', bbox_inches='tight',  pad_inches = 0.1)
    f2.savefig('exclude_diagnal_plots/Without_MIS.pdf', bbox_inches='tight',  pad_inches = 0.1)

def draw_solving_time_vs_solver_type(solve_df):
    print("Drawing solving time vs solver type.")
    solve_df = solve_df.loc[solve_df['Timeout'] == False]
    solve_df = solve_df.loc[solve_df['OutOfMemory'] == False]

    greedy = solve_df[solve_df['SolverType'].str.contains('Greedy')]
    ilp = solve_df[solve_df['SolverType'].str.contains('ILP')]

    f1, axs1 = plt.subplots(figsize=hlp.get_figsize(wf=3.2, hf=0.3))
    axs1.set(xlabel='Cinema Size', ylabel='Average Solving Time (Seconds)')
    sns.lineplot(data=greedy, x="Size", y="SolvingTime", hue="display_name", ax=axs1, ci=False, marker='o')
    plt.legend(title='Solver Type')
    f1.savefig('exclude_diagnal_plots/solving_times_greedy.pdf', bbox_inches='tight', pad_inches = 0.1,)

    f2, axs2 = plt.subplots(figsize=hlp.get_figsize(wf=3.2, hf=0.3))
    axs2.set(xlabel='Cinema Size', ylabel='Average Solving Time (Seconds)')
    sns.lineplot(data=ilp, x="Size", y="SolvingTime", hue="display_name", ax=axs2, ci=False, marker='o')
    plt.legend(title='Solver Type')
    f2.savefig('exclude_diagnal_plots/solving_times_ilp.pdf', bbox_inches='tight', pad_inches = 0.1, )

def count_timeouts(random_df, random_without_df):
    include_ilp = random_df[random_df['SolverType'] == "INCL_DIAG_ILP"]
    include_ilp_mis = random_df[random_df['SolverType'] == "INCL_DIAG_ILP_MIS"]
    exclude_ilp = random_without_df[random_without_df['SolverType'] == "EXCL_DIAG_ILP"]
    exclude_ilp_mis = random_without_df[random_without_df['SolverType'] == "EXCL_DIAG_ILP_MIS"]

    include_timeout_ilp = include_ilp.loc[(include_ilp['Timeout'] == True)]
    include_timeout_ilp_mis = include_ilp_mis.loc[(include_ilp_mis['Timeout'] == True)]
    exclude_timeout_ilp = exclude_ilp.loc[(exclude_ilp['Timeout'] == True)]
    exclude_timeout_ilp_mis = exclude_ilp_mis.loc[(exclude_ilp_mis['Timeout'] == True)]
    
    group_include_ilp = include_timeout_ilp.groupby(['Size']).size().sum()
    group_include_ilp_mis = include_timeout_ilp_mis.groupby(['Size']).size().sum()
    group_exclude_ilp = exclude_timeout_ilp.groupby(['Size']).size().sum()
    group_exclude_ilp_mis = exclude_timeout_ilp_mis.groupby(['Size']).size().sum()

    fig, ax2 = plt.subplots()
    y = np.array([group_include_ilp, group_include_ilp_mis, group_exclude_ilp, group_exclude_ilp_mis])
    labels = ["INCL_DIAG_SEAT_ILP", "INCL_DIAG_SEAT_ILP_IS", "EXCL_DIAG_SEAT_ILP", "EXCL_DIAG_SEAT_ILP_IS"]
    colors = sns.color_palette('Oranges')[0:4]
    plt.pie(y, labels=['','', '', ''], startangle = 90, colors = colors, autopct='%.0f%%')
    ax2.legend(labels, loc="lower left", title="Distance_Solver Type")
    ax2.axis('equal')
    ax2.set_ylabel('')
    
    fig.savefig('exclude_diagnal_plots/time_out.pdf', bbox_inches='tight', pad_inches = 0.1)

if not os.path.exists("exclude_diagnal_plots/"):
    os.makedirs("exclude_diagnal_plots/")

random_df = hlp.getRandomSolverDf()
random_df[~random_df['SolverType'].isin(["MADS_Greedy", "MADS_ILP"])]
random_df["Distance_Constraints"] = "INCL_DIAG"
random_df['SolverType'] = random_df['SolverType'].apply(lambda value: update_solver_type(value, "INCL_DIAG"))

random_without_df = hlp.getExcludeDiagnalData()
random_without_df["Distance_Constraints"] = "EXCL_DIAG"
random_without_df['SolverType'] = random_without_df['SolverType'].apply(lambda value: update_solver_type(value, "EXCL_DIAG"))

#t1 = random_df.loc[random_df['Timeout'] == Tru]e
#t2 = random_without_diagnal.loc[random_without_diagnal['Timeout'] == True]

#print(t1["SolvingTime"])
#print(t2["SolvingTime"])

df = pd.concat([random_df, random_without_df], ignore_index=True)

draw_diagnal_vs_without_diagnal(df)
#draw_pcs_boxplots(random_without_diagnal)
draw_solving_time_vs_solver_type(random_df)

count_timeouts(random_df, random_without_df)