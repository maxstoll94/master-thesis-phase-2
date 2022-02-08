
# img_viewer.py

import PySimpleGUI as sg
import os.path
import networkx as nx
import matplotlib.pyplot as plt
import matplotlib.patches as patches

from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from pathlib import Path

def draw_figure(canvas, figure):
    figure_canvas_agg = FigureCanvasTkAgg(figure, canvas)
    figure_canvas_agg.draw()
    figure_canvas_agg.get_tk_widget().pack(side='top', fill='both', expand=1)
    return figure_canvas_agg

def map_label_to_color(mis, index, label):
    if index in mis:
        return 'tab:green'
    if label == 's':
        return 'tab:gray'
    elif label == 'o':
        return 'tab:red'
    elif label == 'e':
        return 'tab:blue'
    else:
        return 'w'

def map_weight_to_color(weight):
    if (weight == 2):
        return 'tab:red'
    else:
        return 'tab:green'

def generateGraph(graphFile, showMis, showNodeIndex, showEdges):
    plt.figure(figsize=(50,50))
    plt.title(Path(graphFile).stem)

    mis = []
    G = nx.readwrite.graphml.read_graphml(graphFile)

    if showMis:
        for (key, value) in nx.get_node_attributes(G,'isInMIS').items():
            # Check if key is even then add pair to new dictionary
            if value:
                mis.append(key)


    posAsString = nx.get_node_attributes(G, 'pos')

    posAsTuple = {}

    for (key, value) in posAsString.items():
        coordinates = value.split(",")
        x = int(coordinates[0])
        y = int(coordinates[1])

        if (x % 2) == 0:
            y = y + 0.05

        posAsTuple[key] = (x,y)


    node_labels_dict = nx.get_node_attributes(G,'label')
    node_color_dict = {k:map_label_to_color(mis,k,v) for (k,v) in node_labels_dict.items()}

    edge_weight_dict = nx.get_edge_attributes(G,'weight')
    edge_color_dict = {k:map_weight_to_color(v) for (k,v) in edge_weight_dict.items()}

    if showNodeIndex:
        nx.draw(G, posAsTuple, edge_color=edge_color_dict.values(), node_color=node_color_dict.values(), with_labels = True,  linewidths=1,  node_size = 1200, width=2.5)
    else:
        nx.draw_networkx_nodes(G, posAsTuple, node_color=node_color_dict.values(), node_size = 1200, linewidths=1)
        if showEdges:
            nx.draw_networkx_edges(G, posAsTuple, edge_color=edge_color_dict.values(), width=2.5)
        nx.draw_networkx_labels(G, posAsTuple, node_labels_dict, font_size=16)

    plt.tick_params(left=True, bottom=True, labelleft=True, labelbottom=True)
    plt.title("")
    #plt.savefig(graphFileName.replace('graph.xml', 'graph.png'), dpi=1200,  bbox_inches='tight')
    plt.show(block=False)


file_list_column = [
    [
        sg.Text("Graph Folder"),
        sg.In(size=(25, 5), enable_events=True, key="-GRAPH FILE-"),
        sg.FileBrowse(),
        sg.Checkbox('Show Node Index', default=False,  key="-SHOW INDEX-"),
        sg.Checkbox('Show MIS', default=False,  key="-SHOW MIS-"),
        sg.Checkbox('Show Edges', default=False,  key="-SHOW EDGES-"),
        sg.Button('Generate', key="-GENERATE-")
    ]
]

txt_viewer_column = [
    [sg.Multiline(size=(40, 60), key="-TOUT-")]
]

# ----- Full layout -----
layout = [
    [
        sg.Column(file_list_column)
    ]
]

window = sg.Window("Graph Viewer", layout, finalize=True, resizable=True, location=(100, 100), element_justification="center")
graphFileName = ""

# Run the Event Loop
while True:
    event, values = window.read()
  
    if event == "Exit" or event == sg.WIN_CLOSED:
        break
    elif event == "-GRAPH FILE-":  # A file was chosen from the listbox
            graphFileName = values["-GRAPH FILE-"]
    elif event == '-GENERATE-':
        generateGraph(graphFileName, values["-SHOW MIS-"],values["-SHOW INDEX-"], values["-SHOW EDGES-"] )
window.close()