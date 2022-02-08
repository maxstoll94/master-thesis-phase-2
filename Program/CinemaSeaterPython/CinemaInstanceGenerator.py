
import numpy as np

import random
import os

debug = True

perc_empty_array = [0,0.2,0.4,0.6,0.8]
perc_occupied_array = [0.3, 0.5,0.7, 0.9 ]
#                 30,40,50, 60, 70,80,90, 100, 110, 120, 130, 140, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2250, 2500, 2,750, 3000, 
number_of_rows = [5, 8, 10, 12, 5, 5, 18, 10, 11, 12, 10, 14, 15, 20, 10, 20, 20, 30, 20, 20, 45, 25, 110, 10, 20, 70, 20, 32, 34, 50, 50, 50, 50, 50, 55, 60]
number_of_cols = [6, 5, 5, 5, 14, 16, 5,  10, 10, 10, 13, 10, 10, 10, 30, 20, 25, 20, 35, 40, 20, 40, 10, 120, 65, 20, 75, 50, 50, 36, 38, 40, 45, 50, 50, 50]
repeat = 5

#perc_empty_array = [0]
#perc_occupied_array = [0.3]
#number_of_seats = [5]


#n_rows = 150
#n_columns = 150
#n_columns_break = 0
#n_rows_break = 0
#perc_empty = 0.3
#perc_seats_occupied = 0.46
#online = False

#if not debug:
#    n_rows = int(input("Number of rows: "))
#    n_columns = int(input("Number of columns: "))
#    n_rows_break = int(input("Every n seats add an empty row: "))
#    n_columns_break = int(input("Every n seats add an empty column: "))
#    perc_empty = float(input("Percentage of empty spaces: "))
#    perc_seats_occupied = float(input("Percentage of seats occupied: "))
#    online = bool(input("Online(boolean):"))


def generate(n_rows, n_columns, perc_empty):
    cinema = np.full((n_rows, n_columns), 1)

    globalMax = 0
    localMax = 0

    for i in range(n_rows):
    
        for j in range(n_columns):
            if cinema[i, j] == 1:
                temp = random.random()
                if(temp < perc_empty):
                    cinema[i, j] = 0                
                    if localMax > globalMax:
                        globalMax = localMax
                    localMax = 0
                else:
                    localMax = localMax + 1
        if localMax > globalMax:
                    globalMax = localMax
        localMax = 0

    return (cinema, globalMax)


def pretty_print(cinema):
    res = ""
    for i in range(len(cinema)):
        for j in range(len(cinema[i])):
            res += str(cinema[i, j])
        res += "\n"
    return res


def write_to_file(cinema, n_rows, n_columns, group_sizes, fileName):
    pretty_cinema = pretty_print(cinema)
    f = open(fileName, "w")
    f.write(str(n_rows) + "\n")
    f.write(str(n_columns) + "\n")
    f.write(pretty_cinema)

    for group in group_sizes:
        f.write(str(group) + " ")


def count_seats(cinema):
    n_of_people = 0
    for i in range(len(cinema)):
        for j in range(len(cinema[i])):
            if (cinema[i, j] == 1):
                n_of_people += 1
    return n_of_people


def generate_groups_offline(cinema, n_columns, perc_seats_occupied):
    group_sizes = np.full(8, 0)
    n_of_people = 0
    n_of_seats = round(perc_seats_occupied * count_seats(cinema))

    while(n_of_people < n_of_seats):
        rand = random.random()
        remaining_diff = n_of_seats - n_of_people

        print(remaining_diff)
        if rand < 0.2:
            if n_columns >= 1 and remaining_diff >= 1:
                group_sizes[0] += 1
                n_of_people += 1
        elif rand < 0.8:
             if n_columns >= 2 and remaining_diff >= 2:
                group_sizes[1] += 1
                n_of_people += 2
        elif rand < 0.9:
             if n_columns >= 3 and remaining_diff >= 3:
                group_sizes[2] += 1
                n_of_people += 3
        elif rand < 0.95:
             if n_columns >= 4 and remaining_diff >= 4:
                group_sizes[3] += 1
                n_of_people += 4
        elif rand < 0.97:
             if n_columns >= 5 and remaining_diff >= 5:
                group_sizes[4] += 1
                n_of_people += 5
        elif rand < 0.98:
            if n_columns >= 6 and remaining_diff >= 6:
                group_sizes[5] += 1
                n_of_people += 6
        else:
            if n_columns >= 8 and remaining_diff >= 8:
                group_sizes[7] += 1
                n_of_people += 8

    return group_sizes

def generate_list():

    for perc_empty in perc_empty_array:
        for perc_occupied in perc_occupied_array:
            for index in range(0, len(number_of_cols)):
                    n_col = number_of_cols[index]
                    n_row = number_of_rows[index]
                    if not os.path.exists(f"instances/sizes/new/{n_col * n_row}"):
                        os.makedirs(f"instances/sizes/new/{n_col * n_row}")
                    for r in range(repeat):
                     
                        (cinema, maxSize) = generate(n_row, n_col, perc_empty)
                        groups = generate_groups_offline(cinema, maxSize, perc_occupied)
                        fileName = f'random_nrc_{n_col}_{n_row}_pe_{perc_empty * 100}%_po_{perc_occupied * 100}%_{r + 1}.txt'
                        write_to_file(cinema, n_row, n_col, groups, f"instances/sizes/new/{n_col * n_row}/{fileName}")
                        print(f"{fileName}")


#generate_list()

def read_instance(filename="instances/instance.txt"):
    with open(filename, "r") as f:
        lines = f.readlines()
        h = int(lines[0].strip())
        v = int(lines[1].strip())
        people = {}
        problem = np.zeros((h, v)).astype(int)

        for amt, i in enumerate(lines[-1].split()):
            people[amt] = int(i)
        for i, line in enumerate(lines[2:-1]):
            problem[i, :] = np.array([int(z) for z in line.strip()])

    return problem, people, h, v

def generate_exact():
    file_names = ["Arena_2", "Maastricht_2", "Spuimarkt_2", "Tilburg_4", "Ede_9"]
    for file_name in file_names:      
        os.makedirs(f"U:/master thesis/project/Experiment/Exact_Instances/Real_Instances/{file_name}")     
        for perc_occupied in perc_occupied_array:     
            cinema, people, h, v = read_instance(f'U:/master thesis/project/Experiment/Exact_Instances/Real_Instances/{file_name}.txt')
            groups = generate_groups_offline(cinema, 8, perc_occupied)
            fileName = f'{perc_occupied}.txt'
            write_to_file(cinema, h, v, groups, f"U:/master thesis/project/Experiment/Exact_Instances/Real_Instances/{file_name}/{fileName}")


generate_exact()

print("Done")