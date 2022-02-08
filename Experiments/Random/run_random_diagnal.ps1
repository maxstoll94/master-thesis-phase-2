$script:Timeout = 300
$script:GraphFolder = "./Results/Exclude_Diagnal_Graphs"
$script:BaseInstanceFolder = "./Random_Instances"

# $script:number_of_cinemas = 8
# $script:number_of_rows =  11, 12, 10, 14, 15, 20, 10, 20
# $script:number_of_cols =  10, 10, 13, 10, 10, 10, 30, 20

$script:number_of_cinemas = 36
$script:number_of_rows =  5, 8, 10, 12, 5, 5, 18, 10, 11, 12, 10, 14, 15, 20, 10, 20, 20, 30, 20, 20, 45, 25, 110, 10, 20, 70, 20, 32, 34, 50, 50, 50, 50, 50, 55, 60
$script:number_of_cols =  6, 5, 5, 5, 14, 16, 5,  10, 10, 10, 13, 10, 10, 10, 30, 20, 25, 20, 35, 40, 20, 40, 10, 120, 65, 20, 75, 50, 50, 36, 38, 40, 45, 50, 50, 50
#$script:number_of_rows =  14
#$script:number_of_cols =  10

for ($i = 0; $i -lt $number_of_cinemas; $i += 1) {
  $script:row = $number_of_rows[$i]
  $script:col = $number_of_cols[$i]
  $script:size = $row * $col
  $script:name = "random_without_diagnal_" + $size
  $script:folder = $BaseInstanceFolder + "/" + $size

  #Construction
  # Write-Output "Constructing the graphs"
  # ./CinemaSeaterRunner.exe -n "$name" -m "Experiments" -mis "ILP" -i $folder  -g $GraphFolder  -to $Timeout -e "Construct" -x
  # ./CinemaSeaterRunner.exe -n "$name" -m "Experiments" -mis "None" -i $folder  -g $GraphFolder  -to $Timeout -e "Construct" -x
  #
  # #Greedy algorithms
  # Write-Output "Running greedy algorithms"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder  -to $Timeout -e "Solve" -st "Greedy_LF" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder  -to $Timeout -e "Solve" -st "Greedy_SF" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_Random" -mis "None"
  #
  # #Greedy MIS algorithms
  # Write-Output "Running greedy-mis algorithms"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_LF" -mis "ILP"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_SF" -mis "ILP"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_Random" -mis "ILP"

  #ILP
  Write-Output "Running ILP algorithms"

  if ($size -le 150)
  {
    ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "ILP" -mis "None"
  }

  if ($size -le 600)
  {
    ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "ILP_MIS" -mis "ILP"
  }
}

Read-Host -Prompt "Press Enter to exit"
