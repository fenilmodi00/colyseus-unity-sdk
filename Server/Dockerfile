# =============================================================================
# Multi-stage Dockerfile for Colyseus Unity SDK Server
# Optimized for Akash Network deployment
# =============================================================================

# -----------------------------------------------------------------------------
# Build Stage - TypeScript compilation and dependency installation
# -----------------------------------------------------------------------------
FROM node:20-alpine AS builder

# Install build dependencies (minimal set for faster builds)
RUN apk add --no-cache python3 make g++ git

# Set working directory
WORKDIR /app

# Copy package files for dependency resolution
COPY package*.json tsconfig.json ./

# Install ALL dependencies (including devDependencies for build)
RUN npm ci --include=dev --prefer-offline

# Copy source code
COPY src/ ./src/
COPY loadtest/ ./loadtest/

# Build TypeScript to JavaScript
RUN npm run build

# Install only production dependencies in clean directory
RUN mkdir /prod && cd /prod && \
    cp /app/package*.json . && \
    npm ci --omit=dev --cache /tmp/.npm --prefer-offline

# -----------------------------------------------------------------------------
# Production Stage - Minimal runtime image
# -----------------------------------------------------------------------------
FROM node:20-alpine AS production

# Install only essential runtime dependencies for Akash deployment
RUN apk add --no-cache \
    tini \
    curl \
    ca-certificates \
    dumb-init && \
    rm -rf /var/cache/apk/* && \
    adduser -D -s /bin/sh -h /app colyseus

# Set working directory
WORKDIR /app

# Copy production dependencies from builder
COPY --from=builder /prod/node_modules ./node_modules
COPY --from=builder /app/package*.json ./

# Copy built application
COPY --from=builder /app/build ./build

# Copy loadtest directory for performance testing
COPY --from=builder /app/loadtest ./loadtest

# Create logs directory and set ownership
RUN mkdir -p logs tmp && \
    chown -R colyseus:colyseus /app

# Switch to non-root user
USER colyseus

# **CRITICAL FIX**: Only expose the WebSocket port that Akash will map
EXPOSE 2567

# Health check - optimized for Akash Network container orchestration
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
    CMD curl -f http://localhost:2567/health || exit 1

# Use dumb-init for proper signal handling in containers
ENTRYPOINT ["dumb-init", "--"]
CMD ["node", "build/index.js"]
