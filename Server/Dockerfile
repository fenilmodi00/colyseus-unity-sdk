# Colyseus Unity SDK Server - Simplified Docker Image
FROM node:20-alpine

# Install system dependencies
RUN apk add --no-cache curl tini python3 make g++ git

# Create app directory
WORKDIR /app

# Copy package files
COPY package*.json tsconfig.json ./

# Install dependencies and build
RUN npm install

# Copy source code
COPY src/ ./src/
COPY loadtest/ ./loadtest/

# Build TypeScript
RUN npm run build

# Create non-root user and logs directory
RUN adduser -D -s /bin/sh colyseus && \
    mkdir -p logs && \
    chown -R colyseus:colyseus /app
USER colyseus

# Expose ports (2567 for WebSocket and HTTP)
EXPOSE 2567

# Health check
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
    CMD curl -f http://localhost:2567/health || exit 1

# Start server
ENTRYPOINT ["tini", "--"]
CMD ["node", "build/index.js"]