
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

params = {'backend': 'Agg',
           'axes.labelsize': 10,
           } # extend as needed
plt.rcParams.update(params)
plt.rcParams['legend.title_fontsize'] = 'small'
plt.rcParams['legend.fontsize'] = 'small'

def draw_pcs_boxplots(solve_df):
    print("Drawing box plots for PCS.")
    withMIS = solve_df.loc[solve_df['MISType'] != 'None'].sort_values('display_name')
    withoutMIS = solve_df.loc[(solve_df['MISType'] == 'None')].sort_values('display_name')

    withMISg = withMIS.groupby('display_name')
    withoutMISg = withoutMIS.groupby('display_name')
    
    f1, (ax1) = plt.subplots(figsize=hlp.get_figsize(wf=2.1, hf=0.65))
    ax1.set(ylim=(-0.1, 1.1))

    b1 = sns.boxplot(x="display_name", y="PercentageSeated", hue="MISType", data=withMIS, ax=ax1)
    b1.set_xticklabels(['%s\n($n$=%d)'%(k, len(v)) for k, v in withMISg])
    ax1.set(xlabel='Solver Type', ylabel='PCS')
    ax1.legend(title='IS Type')

    f2, (ax3) = plt.subplots(figsize=hlp.get_figsize(wf=2.1, hf=0.65))
    ax3.set(ylim=(-0.1, 1.1))

    b2 = sns.boxplot(x="display_name", y="PercentageSeated",data=withoutMIS, ax=ax3, hue="size_label")
    b2.set_xticklabels(['%s\n($n$=%d)'%(k, len(v)) for k, v in withoutMISg])
    ax3.set(xlabel='Solver Type', ylabel='PCS')
    ax3.legend(title='Cinema Size')

    hlp.add_median_labels(ax1, '.3f')
    hlp.add_median_labels(ax3, '.3f')

    f1.savefig('random_plots/With_MIS.pdf', bbox_inches='tight',  pad_inches = 0.1)
    f2.savefig('random_plots/Without_MIS.pdf', bbox_inches='tight',  pad_inches = 0.1)

def test_greedy_vs_ilp_mis(solve_df):
    print("Testing for greedy vs ilp mis.")
    for alg in hlp.mis_types:
        ilpMIS = solve_df.loc[solve_df['MISType'] == 'ILP']
        greedyMIS = solve_df.loc[solve_df['MISType'] == 'Greedy']

        ilpMIS = ilpMIS.loc[ilpMIS['SolverType'] == alg]
        greedyMIS = greedyMIS.loc[greedyMIS['SolverType'] == alg]

        t_result = stats.ttest_ind(ilpMIS['PercentageSeated'], greedyMIS['PercentageSeated'], alternative='greater')
    
        if t_result[1] < 0.05:
            print(t_result[1])
            print(f"{alg}: ilpMIS and greedyMIS are different")
        else:
            print(t_result[1])
            print(f"{alg}: ilpMIS and greedyMIS are not different")

    ilpMIS = solve_df.loc[solve_df['MISType'] == 'ILP']
    greedyMIS = solve_df.loc[solve_df['MISType'] == 'Greedy']

    d1 = ilpMIS['PercentageSeated']
    c1 = stats.norm.interval(alpha=0.95, loc=np.mean(d1), scale=stats.sem(d1))
    print(c1)
    d2 = greedyMIS['PercentageSeated']
    c2 = stats.norm.interval(alpha=0.95, loc=np.mean(d2), scale=stats.sem(d2))
    print(c2)


def draw_density_vs_size(solve_df):
    print("Drawing density vs size.")
    algNames = ["Greedy_LF", "Greedy_SF", "Greedy_Random", "Greedy_MIS_LF", "Greedy_MIS_SF",  "Greedy_MIS_Random",  "ILP_MIS", "MADS_Greedy", "MADS_ILP", "ILP"]
    displayNames = ['SEAT_GREEDY_LF', 'SEAT_GREEDY_SF', 'SEAT_GREEDY_RAND', 'SEAT_GREEDY_IS_LF', 'SEAT_GREEDY_IS_SF', 'SEAT_GREEDY_IS_RAND', 'SEAT_GREEDY_MADS', 'SEAT_ILP', 'SEAT_ILP_MADS', 'SEAT_ILP_IS' ]

    f1, axs = plt.subplots(5, 2, figsize=hlp.get_figsize(wf=3.2, hf=1.1))

    for x in range(len(algNames)):
        currentAxis = axs.flat[x]
        algName = algNames[x]
        xlabel = None

        if x == 9 or x == 8:
            xlabel="Cinema Size"
    
        currentAxis.set_title(f"{x + 1}: Solver Type = {displayNames[x]}")
        currentAxis.set(ylim=(0.10, 0.7))
  
        data = solve_df.loc[solve_df['SolverType'] == algName]
        sns.lineplot(data=data, x="Size", y="PercentageSeated", hue="density_label", style="density_label", ci='sd', ax=currentAxis, marker='o').set( ylabel="Avg. PCS", xlabel=xlabel)
        currentAxis.legend(loc='lower right', title='Density Type')

    f1.tight_layout(pad=1.2)
    f1.savefig(f'random_plots/density.pdf', bbox_inches='tight', pad_inches = 0.1)

def draw_occupancy_vs_pps(solve_df):
    print("Drawing occupancy vs PPS.")
    f, axs = plt.subplots(figsize=hlp.get_figsize(wf=3.3, hf=0.25))

    bar1 = sns.barplot(data=solve_df, x="display_name", y="PercentagePeopleSeated", hue="occupancy_label", ci=False, palette="Blues")
    bar1.set(xlabel='Solver Type', ylabel='Avg. PPS')
 

    for p in bar1.patches:
        bar1.annotate(round(p.get_height(), 2), 
                       (p.get_x() + p.get_width() / 2., p.get_height()), 
                       ha = 'center', va = 'center', 
                       xytext = (0, 5), 
                       textcoords = 'offset points')
    bar1.legend(title="Occupancy Type")
    #axs.legend(loc='lower right')

    f.savefig(f'random_plots/occupancy_bar.pdf', bbox_inches='tight', pad_inches = 0.1)

def test_greedy_group_order(solver_df):
    print("Testing for greedy group order.")
    algs = ["Greedy_LF","Greedy_Random","Greedy_SF"]
    for alg in algs:
        data = solver_df.loc[solver_df['SolverType'] == alg]
        data = data['PercentageSeated']
        ci = stats.norm.interval(alpha=0.95, loc=np.mean(data), scale=stats.sem(data))

        print(alg)
        print(data.mean())
        print(ci)

def test_greedy_iteration_order(solver_df):
    print("Testing for greedy group order.")
    algs = ["Greedy_MIS_LF","Greedy_MIS_Random","Greedy_MIS_SF"]
    solver_df = solver_df.loc[solver_df['MISType'] == "ILP"]
    for alg in algs:
        data = solver_df.loc[solver_df['SolverType'] == alg]
        data = data['PercentageSeated']
        ci = stats.norm.interval(alpha=0.95, loc=np.mean(data), scale=stats.sem(data))

        print(alg)
        print(data.mean())
        print(ci)

def draw_revenue(solve_df):
    print("Drawing revenue.")
    fig, ax1 = plt.subplots(figsize=hlp.get_figsize(wf=3.2, hf=0.3))

    solve_df["Diff"] = (solve_df["Revenue"] / solve_df["MaxRevenue"])
    solve_g = solve_df.groupby(["display_name", "Size"], as_index=False)["Diff"].mean()
    data = solve_g.pivot("display_name", "Size", "Diff")
    data.index = pd.CategoricalIndex(data.index, categories= ['SEAT_GREEDY_LF', 'SEAT_GREEDY_SF', 'SEAT_GREEDY_RAND', 'SEAT_GREEDY_IS_LF', 'SEAT_GREEDY_IS_SF', 'SEAT_GREEDY_IS_RAND', 'SEAT_GREEDY_MADS', 'SEAT_ILP_IS', 'SEAT_ILP', 'SEAT_ILP_MADS', ])
    data.sort_index(level=0, inplace=True)
    ax = sns.heatmap(data, ax=ax1, linewidths=.5, annot=True, fmt=".3f", annot_kws={"fontsize":7})
    ax.set(xlabel='Cinema Size', ylabel='Solver Type')
    fig.savefig('random_plots/revenue.pdf', bbox_inches='tight', pad_inches = 0.1)

def draw_mis_time(solve_df):
    print("Drawing density time.")
    greens = sns.color_palette("summer")[0:3]
    blues = sns.color_palette("Blues")[0:4]
    ilp = solve_df[solve_df['SolverType'].str.contains('ILP')]

    fig, ax1 = plt.subplots()
    df_group = ilp.groupby(['density_label'])['SolvingTime'].mean()
    labels = ['Very Dense (80 - 100 % seats)', 'Dense (40 - 80 % seats)', 'Sparse (20 - 40 % seats)']
    df_group.plot(kind='pie', labels=['','','',''], colors=greens,  autopct='%.1f%%',  ax=ax1, pctdistance=0.65)
    ax1.legend(labels, loc="best", title="Density Type")
    ax1.axis('equal')
    ax1.set_ylabel('')

    fig.savefig('random_plots/density_ilp_time.pdf', bbox_inches='tight', pad_inches = 0.1)

    fig, ax2 = plt.subplots()
    df_group = ilp.groupby(['occupancy_label'])['SolvingTime'].mean()
    labels = ['Low Occupancy (~30% of seats occupied)',  'Medium Occupancy (~50% of seats occupied)', 'High Occupancy (~70% of seats occupied)', 'Very High Occupancy (~90% of seats occupied)']
    df_group.plot(kind='pie', labels=['','','',''], colors=blues,  autopct='%.1f%%', startangle = 90,  ax=ax2)
    ax2.legend(labels, loc="lower left", title="Occupancy Type")
    ax2.axis('equal')
    ax2.set_ylabel('')

    fig.savefig('random_plots/occupancy_ilp_time.pdf', bbox_inches='tight', pad_inches = 0.1)

    greedy = solve_df[solve_df['SolverType'].str.contains('Greedy')]

    fig, ax1 = plt.subplots()
    df_group = greedy.groupby(['density_label'])['SolvingTime'].mean()
    colors = sns.color_palette("summer")[0:4]
    labels = ['Very Dense (80 - 100 % seats)', 'Dense (40 - 80 % seats)', 'Sparse (20 - 40 % seats)']
    df_group.plot(kind='pie', labels=['','','',''], colors=greens,  autopct='%.1f%%', startangle = 90,  ax=ax1, normalize=True)
    ax1.legend(labels, loc="lower left", title="Density Type")
    ax1.axis('equal')
    ax1.set_ylabel('')

    fig.savefig('random_plots/density_greedy_time.pdf', bbox_inches='tight', pad_inches = 0.1)

    fig, ax2 = plt.subplots()
    df_group = greedy.groupby(['occupancy_label'])['SolvingTime'].mean()
    colors.reverse()
    labels = ['Low Occupancy (~30% of seats occupied)',  'Medium Occupancy (~50% of seats occupied)', 'High Occupancy (~70% of seats occupied)', 'Very High Occupancy (~90% of seats occupied)']
    df_group.plot(kind='pie', labels=['','','',''], colors=blues,  autopct='%.1f%%', startangle = 90,  ax=ax2, normalize=True)
    ax2.legend(labels, loc="lower left", title="Occupancy Type")
    ax2.axis('equal')
    ax2.set_ylabel('')

    fig.savefig('random_plots/occupancy_greedy_time.pdf', bbox_inches='tight', pad_inches = 0.1)

def draw_width_height(solve_df):
    print("Drawing width vs height.")
    fig, ax1 = plt.subplots(figsize=hlp.get_figsize(wf=3.5, hf=0.3))
    bar1 = sns.barplot(data=solve_df, x="display_name", y="PercentageSeated", hue="Size Ratio", ci=95, palette="Purples")
    bar1.set(xlabel='Solver Type', ylabel='Avg. PPS')

    for p in bar1.patches:
        bar1.annotate(round(p.get_height(), 3), 
                       (p.get_x() + p.get_width() / 2., p.get_height()), 
                       ha = 'center', va = 'center', 
                       xytext = (0, 5), 
                       textcoords = 'offset points')
    fig.savefig('random_plots/width_vs_height.pdf', bbox_inches='tight', pad_inches = 0.1)


   
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
    f1.savefig('random_plots/solving_times_greedy.pdf', bbox_inches='tight', pad_inches = 0.1,)

    f2, axs2 = plt.subplots(figsize=hlp.get_figsize(wf=3.2, hf=0.3))
    axs2.set(xlabel='Cinema Size', ylabel='Average Solving Time (Seconds)')
    sns.lineplot(data=ilp, x="Size", y="SolvingTime", hue="display_name", ax=axs2, ci=False, marker='o')
    plt.legend(title='Solver Type')
    f2.savefig('random_plots/solving_times_ilp.pdf', bbox_inches='tight', pad_inches = 0.1, )

def means(solve_df):
    print("Testing for percentage capacity seated.")

    algs = ['SEAT_GREEDY_LF', 'SEAT_GREEDY_SF', 'SEAT_GREEDY_RAND', 'SEAT_GREEDY_IS_LF', 'SEAT_GREEDY_IS_SF', 'SEAT_GREEDY_IS_RAND', 'SEAT_GREEDY_MADS', 'SEAT_ILP_IS', 'SEAT_ILP', 'SEAT_ILP_MADS']
    for alg in algs:
        data = solve_df.loc[solve_df['display_name'] == alg]
        data = data['PercentageSeated']
        ci = stats.norm.interval(alpha=0.95, loc=np.mean(data), scale=stats.sem(data))

        print(alg)
        print(data.mean())
        print(ci)

    #print(solve_df.groupby("SolverType")['PercentageSeated'].mean())
    #print(solve_df.groupby("SolverType")['SolvingTIme'].mean())

###################################################################################################################

if not os.path.exists("random_plots/"):
    os.makedirs("random_plots/")

solve_df = hlp.getRandomSolverDf()
solve_df = solve_df.loc[solve_df['Timeout'] == False]
solve_df = solve_df.loc[solve_df['OutOfMemory'] == False]
draw_density_vs_size(solve_df)
draw_pcs_boxplots(solve_df)
#test_greedy_vs_ilp_mis(solve_df)
draw_occupancy_vs_pps(solve_df)
draw_solving_time_vs_solver_type(solve_df)
draw_revenue(solve_df)
draw_mis_time(solve_df)
draw_width_height(solve_df)
test_greedy_group_order(solve_df)
test_greedy_iteration_order(solve_df)
means(solve_df)