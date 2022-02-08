$script:Timeout = 900
$script:GraphFolder = "./Results/Exact_Graphs"
$script:BaseInstanceFolder = "./Exact_Instances/Real_Instances"


$script:folder = $BaseInstanceFolder
$script:number_of_cinemas = 1
$script:cinemas = @("Ede_9")

for ($i = 0; $i -lt $number_of_cinemas; $i += 1) {
  $script:cinema = $cinemas[$i]
  $script:folder = $BaseInstanceFolder + '/' + $cinema
  $script:name = "exact_" +  $cinema

  #Construction
  # Write-Output "Constructing the graphs"
  # ./CinemaSeaterRunner.exe -n "$name" -m "Experiments" -mis "ILP" -i $folder  -g $GraphFolder  -to $Timeout -e "Construct"
  # ./CinemaSeaterRunner.exe -n "$name" -m "Experiments" -mis "None" -i $folder  -g $GraphFolder  -to $Timeout -e "Construct"
  #
  # #Greedy algorithms
  # Write-Output "Running greedy algorithms"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder  -to $Timeout -e "Solve" -st "Greedy_LF" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder  -to $Timeout -e "Solve" -st "Greedy_SF" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_Random" -mis "None"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "MADS_Greedy" -mis "None"
  #
  # #Greedy MIS algorithms
  # Write-Output "Running greedy-mis algorithms"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_LF" -mis "ILP"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_SF" -mis "ILP"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "Greedy_MIS_Random" -mis "ILP"
  #
  # #ILP
  # Write-Output "Running ILP algorithms"
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "ILP" -mis "None" -d
  # Start-Sleep -Seconds 5
  # ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "MADS_ILP" -mis "None" -d
  # Start-Sleep -Seconds 5

  ./CinemaSeaterRunner.exe -n $name -m "Experiments" -i $folder  -g $GraphFolder -to $Timeout -e "Solve" -st "ILP_MIS" -mis "ILP" -d
}
Read-Host -Prompt "Press Enter to exit"
