# =============================================================================
# Multi-stage Dockerfile for Colyseus Unity SDK Server
# Optimized for Akash Network deployment with minimal size and enhanced security
# =============================================================================

# -----------------------------------------------------------------------------
# Build Stage - TypeScript compilation and dependency installation
# -----------------------------------------------------------------------------
FROM node:20-alpine AS builder

# Install build dependencies
RUN apk add --no-cache python3 make g++ git

# Set working directory
WORKDIR /app

# Copy package files for dependency resolution
COPY package*.json tsconfig.json ./

# Install ALL dependencies (including devDependencies for build)
RUN npm ci --include=dev

# Copy source code
COPY src/ ./src/
COPY loadtest/ ./loadtest/

# Build TypeScript to JavaScript
RUN npm run build

# Install only production dependencies in clean directory
RUN mkdir /prod && cd /prod && \
    cp /app/package*.json . && \
    npm ci --omit=dev --cache /tmp/.npm

# -----------------------------------------------------------------------------
# Production Stage - Minimal runtime image
# -----------------------------------------------------------------------------
FROM node:20-alpine AS production

# Install only essential runtime dependencies for Akash deployment
RUN apk add --no-cache \
    tini \
    curl \
    ca-certificates && \
    rm -rf /var/cache/apk/* && \
    adduser -D -s /bin/sh -h /app colyseus

# Set working directory
WORKDIR /app

# Copy production dependencies from builder
COPY --from=builder /prod/node_modules ./node_modules
COPY --from=builder /app/package*.json ./

# Copy built application
COPY --from=builder /app/build ./build
COPY --from=builder /app/loadtest ./loadtest

# Create logs directory and set ownership
RUN mkdir -p logs && \
    chown -R colyseus:colyseus /app

# Switch to non-root user
USER colyseus

# Expose ports for Akash Network
EXPOSE 80 2567

# Health check - optimized for Akash Network container orchestration
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Use tini for proper signal handling
ENTRYPOINT ["tini", "--"]
CMD ["node", "build/index.js"]