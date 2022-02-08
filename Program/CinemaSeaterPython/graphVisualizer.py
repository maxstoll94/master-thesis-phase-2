
import requests
import networkx as nx
import matplotlib.pyplot as plt
import matplotlib.patches as patches

import sys

file_path = sys.argv[2]
showNodeIndex = [1]
#file = "../CinemaSeaterRunner/Results/Graphs/random_nrc25_pe30.0%_po30.0%.xml"
#solved_file = "../CinemaSeaterRunner/Results/Solving_results/random_nrc25_pe30.0%_po30.0%_BestFit_graph.xml" 
def map_label_to_color(label):
    if label == 's':
        return 'tab:gray'
    elif label == 'o':
        return 'tab:red'
    elif label == 'e':
        return 'tab:blue'
    else:
        return 'tab:green'

def map_weight_to_color(weight):
    if (weight == 2):
        return 'tab:red'
    else:
        return 'tab:green'

fig, (ax1) = plt.subplots(1, 1)

G = nx.readwrite.graphml.read_graphml(file_path)

posAsString = nx.get_node_attributes(G, 'pos')
posAsTuple = {k: tuple(int(x) for x in v.split(",")) for k, v in posAsString.items()}

node_labels_dict = nx.get_node_attributes(G,'label')
node_color_dict = {k:map_label_to_color(v) for (k,v) in node_labels_dict.items()}

edge_weight_dict = nx.get_edge_attributes(G,'weight')
edge_color_dict = {k:map_weight_to_color(v) for (k,v) in edge_weight_dict.items()}

if showNodeIndex:
    nx.draw(G, posAsTuple, edge_color=edge_color_dict.values(), node_color=node_color_dict.values(), with_labels = True)
else:
    nx.draw_networkx_nodes(G, posAsTuple, node_color=node_color_dict.values(), node_size = 400)
    nx.draw_networkx_edges(G, posAsTuple, edge_color=edge_color_dict.values())
    nx.draw_networkx_labels(G, posAsTuple, node_labels_dict, font_size=16)
plt.show()

##pload = {'instanceFile':'path_arena.txt', 'solve': solve, 'shapeType': shapeType}
#pload = {'InstanceFile':'Exact17.txt', 'solve': solve}
#response = requests.post('http://localhost:35434/seating/offline', json = pload, verify=False)

#fig, (ax1) = plt.subplots(1, 1)


#def handleSeatingResponse(jsonResponse):
#    drawGraph(jsonResponse)
#    #drawUDG(jsonResponse)

#def drawGraph(jsonResponse):
#    matrix = jsonResponse['seatingGraph']['edges']
#    labels = jsonResponse['initialLabels']

#    #valid = jsonResponse['valid']
#    #allSeated = jsonResponse['allSeated']
#    #numberOfNotSeatedGroups = jsonResponse['numberOfNotSeatedGroups']
#    #numberOfNotPeopleSeated = jsonResponse['numberOfPeopleSeated']

#    #plt.title("Valid: " + str(valid) + ', Groups not seated: ' + str(numberOfNotSeatedGroups) + ', People seated: ' + str(numberOfNotPeopleSeated))

#    if solve:
#         labels = jsonResponse['seatingGraph']['labels']

#    mis = jsonResponse['mis']
#    shapes = []
    
#    for shape in jsonResponse['shapeRepresentation']:
#        shapes.append(json.loads(shape))

#    labels_dict = {}

#    for l in range(len(labels)):
#        labels_dict[l] = labels[l]

#    edges = matrix.splitlines()

#    edges.pop(0)
#    edges.pop()
#    numOfNodes = len(edges)
    
#    G = nx.MultiGraph()

#    alternate = False

#    for i in range(numOfNodes):
#        n_color = ''

#        if solve:
#            if labels_dict[i] == 'e':
#                n_color = 'tab:blue'
#            elif labels_dict[i] == 'o':
#                n_color = 'tab:red'
#            elif labels_dict[i] == 's':
#                n_color = 'tab:gray'
#            else:
#                n_color = 'tab:orange'
#        else:
#            if i in mis:
#                n_color = 'tab:green'
#            elif labels_dict[i] == 's':
#                n_color = 'tab:gray'
#            else:
#                n_color = 'tab:blue'

#        x = int(shapes[i]['center']['x'])
#        y = int(shapes[i]['center']['y'])

#        if alternate:
#            y = y + 0.15

#        G.add_node(i, color=n_color,  pos=(x,y))

#        alternate = not alternate

#    for i in range(numOfNodes):
#        for j in range(len(edges[i])):
#            if (i != j):
#                edge = int(edges[i][j])
#                # There exists an edge between vertex i and j
#                if (edge > 0):
#                    e_color = 'tab:green'
#                    if (edge == 2):
#                        e_color = 'tab:red'
#                    if i == 0 and j == 2:
#                        e_color = 'tab:orange'
#                    G.add_edge(i,j, color=e_color,  rad=0.3)

#    nx.set_node_attributes(G, labels_dict, 'label')

#    n_colors = nx.get_node_attributes(G,'color').values()
#    pos=nx.get_node_attributes(G, 'pos')
#    e_colors = nx.get_edge_attributes(G,'color').values()

#    if showNodeIndex:
#        nx.draw(G, pos, edge_color=e_colors, node_color=n_colors, with_labels = True)
#    else:
#        nx.draw_networkx_nodes(G, pos, node_color=n_colors, node_size = 400)
#        nx.draw_networkx_edges(G, pos, edge_color=e_colors)
#        nx.draw_networkx_labels(G, pos, labels_dict, font_size=16)

#def drawUDG(jsonResponse):

#    if shapeType == 'R':
#        rectangles = jsonResponse['rectangles']
#        for i, rectangle in enumerate(rectangles):
#            rect = patches.Rectangle((float(rectangle['bottomLeft']['x']),float(rectangle['bottomLeft']['y'])), 1, 1, linewidth=1, edgecolor='k', facecolor='none')
#            ax1.add_patch(rect)
#            #ax1.annotate(i, ((int(rectangle['center']['x']), (int(rectangle['center']['y']))))) # annoted circles
#    elif shapeType == 'D':
#        disks = jsonResponse['disks']
#        for i, disk in enumerate(disks):
#            circle = plt.Circle((int(disk['x']),int(disk['y'])), float(disk['radius']), fill=False) # plot circles (1)
#            ax1.add_patch(circle)
#            ax1.annotate(i, ((int(disk['x']), (int(disk['y']))))) # annoted circles
#    else:
#        crosses = jsonResponse['crosses']

#        for i, cross in enumerate(crosses):
#            points = [
#                        cross['centerRectangle']['topLeft'], cross['centerRectangle']['topRight'], 
#                        cross['rightRectangle']['topLeft'], cross['rightRectangle']['topRight'], cross['rightRectangle']['bottomRight'], cross['rightRectangle']['bottomLeft'],
#                        cross['centerRectangle']['bottomRight'], cross['centerRectangle']['bottomLeft'],
#                        cross['leftRectangle']['bottomRight'], cross['leftRectangle']['bottomLeft'], cross['leftRectangle']['topLeft'], cross['leftRectangle']['topRight'],
#                        cross['centerRectangle']['topLeft']
#                     ]

#            xs = []
#            ys = []

#            for point in points:
#                xs.append(float(point['x']))
#                ys.append(float(point['y']))

#            plt.plot(xs, ys, color='black')


#handleSeatingResponse(response.json())

#plt.axis("equal")
#plt.axis("on")
#plt.show()