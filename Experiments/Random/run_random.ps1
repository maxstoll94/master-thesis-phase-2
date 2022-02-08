$script:Timeout = 900
$script:GraphFolder = "./Results/Graphs2"
$script:BaseInstanceFolder = "./Random_Instances"

# $script:number_of_cinemas = 8
# $script:number_of_rows =  11, 12, 10, 14, 15, 20, 10, 20
# $script:number_of_cols =  10, 10, 13, 10, 10, 10, 30, 20

$script:number_of_cinemas = 1
$script:number_of_rows =  6
$script:number_of_cols =  5

for ($i = 0; $i -lt $number_of_cinemas; $i += 1) {
  $script:row = $number_of_rows[$i]
  $script:col = $number_of_cols[$i]
  $script:size = $row * $col
  $script:name = "random_" + $size
  $script:folder = $BaseInstanceFolder + '/' + $size

  #Construction
  Write-Output "Constructing the graphs"
  ./CinemaSeaterRunner.exe -n "$name" -m "Experiments" -mis "ILP" -i $folder  -g $GraphFolder  -to $Timeout -e "Construct"
  # ./CinemaSeaterRunner.exe -n "$name" -m "Experiments" -mis "Greedy" -i $folder  -g $GraphFolder  -to $Timeout -e "Construct"
  # ./CinemaSeaterRunner.exe -n "$name" -m "Experiments" -mis "None" -i $folder  -g $GraphFolder  -to $Timeout -e "Construct"

  #Greedy algorithms
  # Write-Output "Running greedy algorithms"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder  -to $Timeout -e "Solve" -st "Greedy_LF" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder  -to $Timeout -e "Solve" -st "Greedy_SF" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_Random" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "MADS_Greedy" -mis "None"
  #
  # #Greedy MIS algorithms
  Write-Output "Running greedy-mis algorithms"
  #./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_LF" -mis "ILP"
  ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_SF" -mis "ILP"
  #./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_Random" -mis "ILP"
  #
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_LF" -mis "Greedy"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_SF" -mis "Greedy"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_Random" -mis "Greedy"

  #ILP
  #Write-Output "Running ILP algorithms"

  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "ILP" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "MADS_ILP" -mis "None"
  #
  # # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "ILP_MIS" -mis "Greedy"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "ILP_MIS" -mis "ILP"
}

Read-Host -Prompt "Press Enter to exit"
