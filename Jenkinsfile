pipeline {
   agent any
   
    environment {
	   scannerHome = tool name: 'SonarQubeScanner'
	   registry = 'utkarshgoyal/samplekubernetes'
	   docker_port = "${env.BRANCH_NAME == 'master'?7200: 7300}"
	   username = 'utkarshgoyal'
	   container_name = "c-utkarshgoyal-${env.BRANCH_NAME}"
	   cluster_name = 'cluster-1'
	   location = 'us-central1-c'
	   credentials_id = 'TestJenkinsApi'
	   project_id = 'grand-sphere-321608'
	}
	options {
		  // prepend all console output generated during statges  
		     timestamps()

		  // Set Timeout period for pipeline run after which jenkins shout abort
		     timeout(time:1, unit: 'HOURS')

	      // Skip checking out code from default 
		   skipDefaultCheckout()

	       buildDiscarder(logRotator(
		     // number of build logs to keep
			 numToKeepStr: '80',
			 // history to keep in days
			 daysToKeepStr: '15'
		   ))		   	  
		}	
	stages {
	   
	   stage('Start') { 
	      steps {
		       checkout scm	   
		  }
	   }
	   
	   stage('nuget restore'){
	     steps {	
		   echo "Nuget Restore Step"
		   bat "dotnet restore"
		 }
	   }
	   
 	   stage('Start sonarqube analysis'){
	        when {
                expression { env.BRANCH_NAME == 'master' }
            }
	     steps {
		     echo "Start sonarqube analysis step"
			 withSonarQubeEnv('Test_Sonar'){
				bat "${scannerHome}/SonarScanner.MSBuild.exe begin /k:SampleWebApp  /d:sonar.cs.opencover.reportsPaths=coverage.opencover.xml"				
			 }
		 }
	   }
	   
	   stage('Code Build'){
	     steps {
		     // clean the output of project
			 echo "clean previous build"
			 bat "dotnet clean"
			 
			 // build the project and all its dependies.
			 echo "Code Build"
			 bat 'dotnet build -c Release -o "SampleWebApp/app/build"'
			 echo 'start Testing'
             bat "dotnet test SampleApplicationTest\\SampleApplicationTest.csproj /p:CollectCoverage=true  /p:CoverletOutputFormat=opencover"// -l:xml;LogFileName=NAGPAPITestOutput.xml"
		 }
	   }
	   
	   stage('Release artifact') {
	   	        when {
                expression { env.BRANCH_NAME == 'develop' }
            }
            steps {
                echo 'release artifact'
                bat 'dotnet publish -c Release'
            }
        }
	   
	   stage('Stop sonarqube analysis'){
	   	     when {
                expression { env.BRANCH_NAME == 'master' }
            }
	      steps {
		     echo "Stop analysis"
			 withSonarQubeEnv('Test_Sonar'){
			   bat "${scannerHome}/SonarScanner.MSBuild.exe end"
			 }
		  }
	   }
	   	   
	   stage('Docker Image'){
	    steps {
		  echo "Docker Image Step"
		  bat 'dotnet publish -c Release'
		  bat "docker build -t i-${username}-${BRANCH_NAME} --no-cache -f Dockerfile ."
		}
	   }
	   
	   stage('Containers'){
	    parallel {
                stage('Pre-Container Check') {
                    steps {
                        echo 'Checking if Container is previously deployed'
                        script {
                            String dockerCommand = "docker ps -a -q -f name=${container_name}"
                            String commandExecution = "${bat(returnStdout: true, script: dockerCommand)}"
                            String docker_previous_containerId = "${commandExecution.trim().readLines().drop(1).join(' ')}"

                            if (docker_previous_containerId != '') {
                                echo "Previous Deploymnet Found. Container Id ${docker_previous_containerId}"

                                echo "Stopping Container ${docker_previous_containerId}"
                                bat "docker stop ${docker_previous_containerId}"

                                echo "Removing Container ${docker_previous_containerId}"
                                bat "docker rm ${docker_previous_containerId}"
                            } else {
                                echo 'Container Not Deployed Previously'
                            }
                        }
                        echo 'Pre-Container Check Complete'
                    }
                }
				stage('PushtoDockerHub')
			   {
				 steps {
					 echo "Move Image to Docker Hub"
					 echo env.containerId
					 bat "docker tag i-${username}-${BRANCH_NAME} ${registry}:${BUILD_NUMBER}"
					 bat "docker tag i-${username}-${BRANCH_NAME} ${registry}:latest-${BRANCH_NAME}"
					 
					 withDockerRegistry([credentialsId: 'DockerHub', url:""]){	  
					   bat "docker push  ${registry}:${BUILD_NUMBER}"
					   bat "docker push  ${registry}:latest-${BRANCH_NAME}"
					 }
				  }
			  }
		   }
	   }	   
	   	   
	   stage('Docker Deployment'){
	     steps{
		   echo "Docker Deployment"
		    bat "docker run --name ${container_name} -d -p ${docker_port}:80 ${registry}:${BUILD_NUMBER}"
		 }
	   }
	   
	   stage('Kubernetes Deployment'){
		 steps{
		   bat " gcloud auth activate-service-account --key-file=grand-sphere-321608-91ff9a48b2a5.json"
		   bat "gcloud container clusters get-credentials cluster-1 --zone us-central1-c --project grand-sphere-321608"
		   bat "kubectl apply -f deployment.yaml"
		 }
	   }	   
	}		
  }