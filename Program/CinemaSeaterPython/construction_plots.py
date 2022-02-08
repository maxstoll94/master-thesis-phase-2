
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

def draw_construction_mis_count(construction_df):
    print("Drawing mis count.")
    withoutNone = construction_df.loc[construction_df['MISType'] != 'None']

    fig = plt.figure(figsize=hlp.get_figsize(wf=3.2, hf=0.3))

    bar = sns.barplot(data = withoutNone, x='size_label', y='MISLength', hue="MISType", palette='summer', ci = False)

    bar.set(
        ylabel="Average MIS Count",
        xlabel="Cinema Size"
    )

    for p in bar.patches:
        bar.annotate(format(round(p.get_height()), '.0f'), 
                       (p.get_x() + p.get_width() / 2., p.get_height()), 
                       ha = 'center', va = 'center', 
                       xytext = (0, 5), 
                       textcoords = 'offset points')

    fig.savefig('construction_plots/mis_count.pdf', bbox_inches='tight', pad_inches = 0.1)

def draw_construction_mis_time(construction_df):
    print("Drawing mis time.")
    fig = plt.figure(figsize=hlp.get_figsize(wf=2, hf=0.3))
    withoutNone = construction_df.loc[construction_df['MISType'] != 'None']

    sns.lineplot(
        data=withoutNone, 
        x="Size", y="MISFindingTime", hue="MISType",
        palette="Set2",
        ci = False
    ).set(
        ylabel="Average MIS Finding Time (Seconds)",
        xlabel="Cinema Size"
    )

    plt.xticks([0, 500, 1000, 1500, 2000, 2500, 3000])

    fig.savefig('construction_plots/mis_time.pdf', bbox_inches='tight', pad_inches = 0.1)

def draw_construction_time(construction_df):
    print("Drawing construction time.")
    fig  = plt.figure(figsize=hlp.get_figsize(wf=2, hf=0.3))

    ct = sns.lineplot(
        data=construction_df, 
        x="Size", y="ConstructionTime", 
        palette="Set2",
        ci=False)

    ct.set(ylabel="Average Construction Time (Seconds)", xlabel="Cinema Size")

    fig.savefig('construction_plots/construction_time.pdf', bbox_inches='tight', pad_inches = 0.1)

###################################################################################################################

if not os.path.exists("construction_plots/"):
    os.makedirs("construction_plots/")

construction_df = hlp.getRandomConstructionDf()

draw_construction_mis_count(construction_df)
draw_construction_mis_time(construction_df)
draw_construction_time(construction_df)

