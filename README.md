
# Sample Kubernetes Jenkins Intergration

This project deplicts the Multibranch pipeline on jenkins
with sonarqube analysis. Sample Web App is docker image
is pushed to a public dockerhub repository and deployed on Google Kubernetes Engine.





![](Screenshots/jenkins-develop.png)
## Tools & Techonologies

```bash
Dotnet Core: Sample WebApp
XUnit: Sample WebApp Unit Testing
Sonarqube: For analysis
Dockerhub: For pushing the docker image 
Kubernetes: Google Kubernetes Engine
```
Ports:
```bash 
For docker master branch: 7200
For docker develop branch: 7300
For GCP master branch: 30157
For GCP docker develop branch: 30158
```
Sample API Example'
```bash 
GET Products: http://localhost:7300/product
```

## Running Kubernetes 
Make sure to expose the port by running the cmd.
```bash 
gcloud compute firewall-rules create test-node-port --allow tcp:30158
gcloud compute firewall-rules create test-npde-port --allow tcp:30157
```




    
