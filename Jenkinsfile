pipeline {
    agent any

    environment {
        PROJECT = 'DotNetDevOpsApp/DotNetDevOpsApp.csproj'
        TEST_PROJECT = 'DotNetDevOpsApp.Tests/DotNetDevOpsApp.Tests.csproj'
        PUBLISH_DIR = 'publish'

        SERVER = 'ubuntu@172.31.28.18'
        REMOTE_DIR = '/opt/dotnetapp'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh '''
                    dotnet restore "$PROJECT" \
                    --runtime linux-x64

                    dotnet restore "$TEST_PROJECT"
                '''
            }
        }

        stage('Build') {
            steps {
                sh '''
                    dotnet build "$PROJECT" \
                        --configuration Release \
                        --no-restore
                '''
            }
        }

        stage('Test') {
            steps {
                sh '''
                    dotnet test "$TEST_PROJECT" \
                        --configuration Release \
                        --no-restore \
                        --logger "console;verbosity=normal"
                '''
            }
        }

        stage('Publish') {
            steps {
                sh '''
                    rm -rf "$PUBLISH_DIR"

                    dotnet restore "$PROJECT" \
                    --runtime linux-x64

                    dotnet publish "$PROJECT" \
                    --configuration Release \
                    --runtime linux-x64 \
                    --self-contained true \
                    --output "$PUBLISH_DIR" \
                    --no-restore

                    chmod +x "$PUBLISH_DIR/DotNetDevOpsApp"

                    echo "Published files:"
                    ls -lh "$PUBLISH_DIR"
                '''
            }
        }

        stage('Deploy') {
            steps {
                sh '''
                    RELEASE_DIR="/tmp/dotnetapp-release-$BUILD_NUMBER"

                    echo "Creating remote release directory..."
                    ssh "$SERVER" \
                        "rm -rf $RELEASE_DIR && mkdir -p $RELEASE_DIR"

                    echo "Copying application to EC2 #2..."
                    scp -r "$PUBLISH_DIR"/. \
                        "$SERVER:$RELEASE_DIR/"

                    echo "Stopping application..."
                    ssh "$SERVER" \
                        "sudo systemctl stop dotnetapp || true"

                    echo "Replacing application files..."
                    ssh "$SERVER" \
                        "sudo rm -rf $REMOTE_DIR/* && \
                         sudo cp -r $RELEASE_DIR/. $REMOTE_DIR/ && \
                         sudo chown -R dotnetapp:dotnetapp $REMOTE_DIR && \
                         sudo chmod +x $REMOTE_DIR/DotNetDevOpsApp"

                    echo "Starting application..."
                    ssh "$SERVER" \
                        "sudo systemctl start dotnetapp"

                    echo "Checking systemd service..."
                    ssh "$SERVER" \
                        "sudo systemctl is-active --quiet dotnetapp"

                    echo "Cleaning temporary release..."
                    ssh "$SERVER" \
                        "rm -rf $RELEASE_DIR"

                    echo "Deployment successful."
                '''
            }
        }

        stage('Health Check') {
            steps {
                sh '''
                    echo "Running application health check..."

                    ssh "$SERVER" \
                        "curl --fail --silent --show-error http://127.0.0.1:5000/health"

                    echo ""
                    echo "Health check passed."
                '''
            }
        }
    }

    post {
        success {
            echo 'CI/CD pipeline completed successfully.'
        }

        failure {
            echo 'CI/CD pipeline failed. Check the stage logs above.'
        }

        always {
            archiveArtifacts artifacts: 'publish/**',
                             fingerprint: true,
                             allowEmptyArchive: true
        }
    }
}
