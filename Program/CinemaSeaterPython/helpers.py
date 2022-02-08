import pandas as pd
import os
import matplotlib.patheffects as path_effects

solver_types = [
    ("Greedy_LF", "None"), ("Greedy_SF", "None"), ("Greedy_Random", "None"),
    ("Greedy_MIS_LF", "Greedy"), ("Greedy_MIS_SF", "Greedy"), ("Greedy_MIS_Random", "Greedy"), ("Greedy_MIS_LF", "ILP"),("Greedy_MIS_SF", "ILP") ,("Greedy_MIS_Random", "ILP"),
    ("MADS_Greedy", "None"), ("MADS_ILP", "None"),
    ("ILP", "None"), 
    ("ILP_MIS", "Greedy"), ("ILP_MIS", "ILP")
]

mis_types = [ "Greedy", "ILP", "None"]

numberOfCinemas = 36
number_of_rows = [5, 8, 10, 12, 5, 5, 18, 10, 11, 12, 10, 14, 15, 20, 10, 20, 20, 30, 20, 20, 45, 25, 110, 10, 20, 70, 20, 32, 34, 50, 50, 50, 50, 50, 55, 60]
number_of_cols = [6, 5, 5, 5, 14, 16, 5,  10, 10, 10, 13, 10, 10, 10, 30, 20, 25, 20, 35, 40, 20, 40, 10, 120, 65, 20, 75, 50, 50, 36, 38, 40, 45, 50, 50, 50]

number_of_repeats = 50

cinemas = ["Maastricht_2", "Arena_2", "Spuimarkt_2", "Tillburg_4", "Ede_9"]

columnwidth = 455.24411


def density(value):
    if value < 0.45:
        return 'sparse'
    elif value < 0.65:
        return 'dense'
    elif value <= 1:
        return 'very dense'

def size(value):
    return str(value)

def occupancy(value):
    if value <= 0.35:
        return 'low occupancy'
    elif value <= 0.55:
        return 'medium occupancy'
    elif value <= 0.75:
        return 'high occupancy'
    else:
        return 'very high occupancy'

def algType(value):
    if "Greedy" in value:
        return "Greedy"
    else:
        return "ILP"

def sizeType(value):
    if value <= 150:
        return "less_than_150"
    else:
        return "more_than_150"

def displayName(value):
    if value == 'Greedy_LF':
        return 'SEAT_GREEDY_LF'

    if value == 'Greedy_SF':
        return 'SEAT_GREEDY_SF'

    if value == 'Greedy_Random':
        return 'SEAT_GREEDY_RAND'

    if value == 'Greedy_MIS_LF':
        return 'SEAT_GREEDY_IS_LF'

    if value == 'Greedy_MIS_SF':
        return 'SEAT_GREEDY_IS_SF'

    if value == 'Greedy_MIS_Random':
        return 'SEAT_GREEDY_IS_RAND'

    if value == 'MADS_Greedy':
        return 'SEAT_GREEDY_MADS'

    if value == 'ILP':
        return 'SEAT_ILP'

    if value == 'MADS_ILP':
        return 'SEAT_ILP_MADS'

    if value == 'ILP_MIS':
        return 'SEAT_ILP_IS'



def getSolverFile(path, solverType, misType, label):
    filepath = f'{path}\solver_{solverType}_{misType}_{label}.csv'
    if (os.path.isfile(filepath)):
        return filepath
    return None

def getConstructionFile(path, misType, label):
    filepath = f'{path}\construction_{misType}_{label}.csv'
    if (os.path.isfile(filepath)):
        return filepath
    return None

def getRandomSolverDf():     
    dfs = []
    for index, tuple in enumerate(solver_types):
        for i in range(numberOfCinemas):
            row = number_of_rows[i]
            col = number_of_cols[i]
            file = getSolverFile("U:\master thesis\project\Experiment\Results\Experiment_results_random", tuple[0], tuple[1], row * col)
            if file != None:
                df = pd.read_csv(file, sep=',')
                dfs.append(df)
    df = pd.concat(dfs, ignore_index=True)
    return add_solve_columns(df)

def getRandomConstructionDf():     
    dfs = []
    for index, mis_type in enumerate(mis_types):
        for i in range(numberOfCinemas):
            row = number_of_rows[i]
            col = number_of_cols[i]

            file = getConstructionFile("U:\master thesis\project\Experiment\Results\Experiment_results_random", mis_type, row * col)

            if file != None:
                df = pd.read_csv(file, sep=',')
                dfs.append(df)
    df = pd.concat(dfs, ignore_index=True)
    return add_construction_columns(df)

def getRandomRepeatData():
    dfs = []
    for index, tuple in enumerate(solver_types):
        for i in range(numberOfCinemas):
            for j in range(number_of_repeats):
                row = number_of_rows[i]
                col = number_of_cols[i]
                file = getSolverFile("U:\master thesis\project\Experiment\Results\Experiment_results_greedy_random", tuple[0], tuple[1], f"random_{row * col}_{j}")
                if file != None:
                    df = pd.read_csv(file, sep=',')
                    df['RepeatId'] = j
                    dfs.append(df)
    df = pd.concat(dfs, ignore_index=True)
    return add_solve_columns(df)

def getExcludeDiagnalData():
    dfs = []
    for index, tuple in enumerate(solver_types):
        for i in range(numberOfCinemas):
                row = number_of_rows[i]
                col = number_of_cols[i]
                file = getSolverFile("U:\master thesis\project\Experiment\Results\Exclude_Diagnal_Experiment_results", tuple[0], tuple[1], f"random_without_diagnal_{row * col}")
                if file != None:
                    df = pd.read_csv(file, sep=',')
                    dfs.append(df)
    df = pd.concat(dfs, ignore_index=True)
    return add_solve_columns(df)

def getVariableData():
    dfs = []
    for index, tuple in enumerate(solver_types):
        for i in range(numberOfCinemas):
                row = number_of_rows[i]
                col = number_of_cols[i]
                file = getSolverFile("U:\master thesis\project\Experiment\Results\Experiment_results", tuple[0], tuple[1], f"random_{row * col}")
                if file != None:
                    df = pd.read_csv(file, sep=',')
                    dfs.append(df)
    df = pd.concat(dfs, ignore_index=True)
    return add_solve_columns(df)

def getExactData():
    dfs = []
    
    for index, tuple in enumerate(solver_types):
        for index, cinema in enumerate(cinemas):
            file = getSolverFile("U:\master thesis\project\Experiment\Results\Experiment_results", tuple[0], tuple[1], f"exact_{cinema}")

            if file != None:
                df = pd.read_csv(file, sep=',')
                df['Cinema_Room'] = cinema
                dfs.append(df)
    df = pd.concat(dfs, ignore_index=True)
    return add_solve_columns(df)

def add_solve_columns(solve_df):
    solve_df['alg_type'] = solve_df['SolverType'].apply(lambda value: algType(value))
    solve_df['alg_type'] = pd.Categorical(solve_df['alg_type'], 
                                           categories=['Greedy', 'ILP'])

    solve_df['size_label'] = solve_df['Size'].apply(lambda value: sizeType(value))
    solve_df['size_label'] = pd.Categorical(solve_df['size_label'], 
                                               categories=['less_than_150', 'more_than_150'])

    solve_df['PercentagePeopleSeated'] = solve_df['NumberOfPeopleSeated']/solve_df['NumberOfPeople']

    solve_df['Revenue'] = solve_df['NumberOfPeopleSeated'].apply(lambda value: value * 12)
    solve_df['MaxRevenue'] = solve_df['NumberOfPeople'].apply(lambda value: value * 12)

    solve_df['density_label'] = solve_df['PercentageEmpty'].apply(lambda value: density(value))
    solve_df['density_label'] = pd.Categorical(solve_df['density_label'], 
                                                categories=['very dense', 'dense', 'sparse'])

    solve_df['SolvingTime'] = pd.to_timedelta(solve_df['SolvingTime']).dt.total_seconds()

    solve_df['occupancy_label'] = solve_df['PercentageOccupied'].apply(lambda value: occupancy(value))
    solve_df['occupancy_label'] = pd.Categorical(solve_df['occupancy_label'], 
                                                categories=['low occupancy', 'medium occupancy', 'high occupancy', 'very high occupancy'])

    solve_df['display_name'] = solve_df['SolverType'].apply(lambda value: displayName(value))

    solve_df['wider'] = solve_df['Width'] > solve_df["Height"]

    solve_df.loc[solve_df['Width'] > solve_df['Height'], 'Size Ratio'] = 'Wide'
    solve_df.loc[solve_df['Width'] < solve_df['Height'], 'Size Ratio'] = 'Narrow' 
    solve_df.loc[solve_df['Width'] == solve_df['Height'], 'Size Ratio'] = 'Square' 

    return solve_df

def add_construction_columns(construction_df):
    construction_df['size_label'] = construction_df['Size'].apply(lambda value: size(value))
    construction_df['ConstructionTime'] = pd.to_timedelta(construction_df['ConstructionTime']).dt.total_seconds()
    construction_df['MISFindingTime'] = pd.to_timedelta(construction_df['MISFindingTime']).dt.total_seconds()
    return construction_df

def get_figsize(wf=0.5, hf=(5.**0.5-1.0)/2.0, ):
    """Parameters:
    - wf [float]:  width fraction in columnwidth units
    - hf [float]:  height fraction in columnwidth units.
                    Set by default to golden ratio.
    - columnwidth [float]: width of the column in latex. Get this from LaTeX 
                            using \showthe\columnwidth
    Returns:  [fig_width,fig_height]: that should be given to matplotlib
    """
    fig_width_pt = columnwidth*wf 
    inches_per_pt = 1.0/72.27               # Convert pt to inch
    fig_width = fig_width_pt*inches_per_pt  # width in inches
    fig_height = fig_width*hf      # height in inches
    return [fig_width, fig_height]

def show_values(axs, orient="v", space=.01):
    def _single(ax):
        if orient == "v":
            for p in ax.patches:
                _x = p.get_x() + p.get_width() / 2
                _y = p.get_y() + p.get_height() + (p.get_height()*0.01)
                value = '{:.1f}'.format(p.get_height())
                ax.text(_x, _y, value, ha="center") 
        elif orient == "h":
            for p in ax.patches:
                _x = p.get_x() + p.get_width() + float(space)
                _y = p.get_y() + p.get_height() - (p.get_height()*0.5)
                value = '{:.1f}'.format(p.get_width())
                ax.text(_x, _y, value, ha="left")

    if isinstance(axs, np.ndarray):
        for idx, ax in np.ndenumerate(axs):
            _single(ax)
    else:
        _single(axs)

def add_median_labels(ax, precision='.2f'):
    lines = ax.get_lines()
    boxes = [c for c in ax.get_children() if type(c).__name__ == 'PathPatch']
    lines_per_box = int(len(lines) / len(boxes))
    for median in lines[4:len(lines):lines_per_box]:
        x, y = (data.mean() for data in median.get_data())
        # choose value depending on horizontal or vertical plot orientation
        value = x if (median.get_xdata()[1] - median.get_xdata()[0]) == 0 else y
        text = ax.text(x, y, f'{value:{precision}}', ha='center', va='center',
                       fontweight='bold', color='white')
        # create median-colored border around white text for contrast
        text.set_path_effects([
            path_effects.Stroke(linewidth=3, foreground=median.get_color()),
            path_effects.Normal(),
        ])
