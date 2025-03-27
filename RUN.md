### Running the Container

1. Build the Docker image:  
   ```sh
   docker build --tag euni-frontend .
   ```

2. Run the container:  
   ```sh
   docker run -p <host-port>:<container-port> euni-frontend
   ```
