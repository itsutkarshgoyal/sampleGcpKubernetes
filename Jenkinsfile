pipeline {
   agent any
   
    environment {
	   //BRANCH_NAME = "${scm.branches[0].name}"
	   scannerHome = tool name: 'SonarQubeScanner'
	   registry = 'utkarshgoyal/samplekubernetes'
	   properties = null
	   docker_port = null
	   username = 'utkarshgoyal'
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
		  bat "docker build -t ${username} --no-cache -f Dockerfile ."
		}
	   }
	   
	   stage('Containers'){
	    parallel {
		 stage('PreContainer Check'){
		 environment
			{
			containerId = "${bat(script: 'docker ps -a -q -f name=c-utkarshgoyal-develop,returnStdout:true').trim().readLines().drop}"
			echo env.containerId
			}
			when {
			  expression{
				return containerId != null	
			  }
			}
			steps {
			  echo "PreContainer Check"
			  bat 'docker ps -f name=c-${registry} -q | xargs --no-run-if-empty docker container stop'
              bat "docker stop c-utkarshgoyal-develop && docker rm c-utkarshgoyal-develop"
			}
		   }
	    stage('PushtoDockerHub')
	   {
	     steps {
		     echo "Move Image to Docker Hub"
			 bat "docker tag ${username} ${registry}:${BUILD_NUMBER}"
			 bat "docker tag ${username} ${registry}:latest"
			 
			 withDockerRegistry([credentialsId: 'DockerHub', url:""]){	  
			   bat "docker push ${registry}:${BUILD_NUMBER}"
			   bat "docker push ${registry}:latest"
			 }
		 }
	   }
		 }
	   }	   
	   
	   stage('Docker Deployment Master'){
	   	   	when {
                expression { env.BRANCH_NAME == 'master' }
            }
	     steps{
		   echo "Docker Deployment in master branch"
		    bat "docker run --name c-${registry} -d -p 7200:80 ${registry}:${BUILD_NUMBER}"
		 }
	   }
	   
	   stage('Docker Deployment'){
	   	 when {
                expression { env.BRANCH_NAME == 'develop' }
          }
	     steps{
		   echo "Docker Deployment in develop branch"
		    bat "docker run --name c-${registry} -d -p 7300:80 ${registry}:${BUILD_NUMBER}"
		 }
	   }
	   
	   /*stage('Kubernetes Deployment'){
	   	   	 when {
                expression { env.BRANCH_NAME == 'master' }
          }
		 steps{
		   step([$class: 'KubernetesEngineBuilder',projectId: env.project_id,clusterName: env.cluster_name,location: env.location, manifestPattern:'deployment.yaml',credentialsId: env.credentials_id, verifyDeployments:true])
		 }
		}*/	   
	}		
  }