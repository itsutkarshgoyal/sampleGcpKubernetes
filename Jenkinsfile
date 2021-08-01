pipeline {
   agent any
   
    environment {
	   scannerHome = tool name: 'SonarQubeScanner'
	   registry = 'utkarshgoyal/samplekubernetes'
	   properties = null
	   docker_port = null
	   username = 'utkarshgoyal'
	   cluster_name = 'autopilot-cluster-1'
	   location = 'us-central1'
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
		  // echo "Running build ${JOB_NAME} # ${BUILD_NUMBER} for ${properties['user.employeeid']} with docker as ${docker_port}"
		   echo "Nuget Restore Step"
		   bat "dotnet restore"
		 }
	   }
 	   stage('Start sonarqube analysis'){
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
			 
			 // build the project and all its dependies
			 echo "Code Build"
			 bat 'dotnet build -c Release -o "SampleWebApp/app/build"'
		 }
	   }
	   
	   stage('Test Case Execution') {
            steps {
                echo 'start Testing'
                 bat "dotnet test SampleApplicationTest\\SampleApplicationTest.csproj /p:CollectCoverage=true  /p:CoverletOutputFormat=opencover"// -l:xml;LogFileName=NAGPAPITestOutput.xml"
            }
        }
	   
	   stage('Stop sonarqube analysis'){
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
		  bat "docker build -t i_${username}_master --no-cache -f Dockerfile ."
		}
	   }
	   
	  /*stage('Building image') {
        steps{
          script {
              docker.build registry + ":$BUILD_NUMBER"
           }
		    bat "docker tag i_${username} ${registry}:latest"
		}
      }*/
	  	  
	    stage('Move Image to Docker Hub')
	   {
	     steps {
		     echo "Move Image to Docker Hub"
			 bat "docker tag i_${username}_master :$BUILD_NUMBER"
			 bat "docker tag i_${username}_master ${registry}:latest"
			 withDockerRegistry([credentialsId: 'DockerHub', url:""]){
               bat "docker tag i_${username} ${registry}:latest"			  
			   bat "docker push ${registry}:${BUILD_NUMBER}"
			   bat "docker push ${registry}:latest"
			 }
		 }
	   }
	   
	   stage('Deploy to GKE'){
		 steps{
		   step([$class: 'kubernetesEngineBuilder',projectId: env.project_id,clusterName: env.cluster_name,location: env.location, manifestPattern:'deployment.yaml',credentialsId: env.credentials_id, verifyDeployments:true])
		 }
		}	   
	}		
    }