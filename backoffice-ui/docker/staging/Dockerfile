FROM node:18-alpine AS base

# Install dependencies only when needed
FROM base AS deps
# Check https://github.com/nodejs/docker-node/tree/b4117f9333da4138b03a546ec926ef50a31506c3#nodealpine to understand why libc6-compat might be needed.
RUN apk add --no-cache libc6-compat
WORKDIR /app

# Config npm token for install packages
RUN echo @pi-financial:registry=https://npm.pkg.github.com >> ~/.npmrc
RUN --mount=type=secret,id=GIT_AUTH_TOKEN \
    echo //npm.pkg.github.com/:_authToken=$(cat /run/secrets/GIT_AUTH_TOKEN) >> ~/.npmrc

# Install dependencies based on the preferred package manager
COPY ../../package.json yarn.lock* package-lock.json* pnpm-lock.yaml* ./
RUN \
  if [ -f yarn.lock ]; then yarn --frozen-lockfile; \
  elif [ -f package-lock.json ]; then npm ci; \
  elif [ -f pnpm-lock.yaml ]; then yarn global add pnpm && pnpm i --frozen-lockfile; \
  else echo "Lockfile not found." && exit 1; \
  fi


# Rebuild the source code only when needed
FROM base AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY ../.. .

# Next.js collects completely anonymous telemetry data about general usage.
# Learn more here: https://nextjs.org/telemetry
# Uncomment the following line in case you want to disable telemetry during the build.
# ENV NEXT_TELEMETRY_DISABLED 1

# This will do the trick, use the corresponding env file for each environment.
COPY .env.staging .env.production
# Add new line before adding new env variables to the file
RUN echo >> .env.production
# Mount and add env variables from Github Secrets
RUN --mount=type=secret,id=DATADOG_APPLICATION_ID \
  echo NEXT_PUBLIC_DATADOG_APPLICATION_ID=$(cat /run/secrets/DATADOG_APPLICATION_ID) >> .env.production
RUN --mount=type=secret,id=DATADOG_CLIENT_TOKEN \
  echo NEXT_PUBLIC_DATADOG_CLIENT_TOKEN=$(cat /run/secrets/DATADOG_CLIENT_TOKEN) >> .env.production
RUN --mount=type=secret,id=DIRECTUS_PROJECT_URL \
  echo NEXT_PUBLIC_DIRECTUS_PROJECT_URL=$(cat /run/secrets/DIRECTUS_PROJECT_URL) >> .env.production
RUN --mount=type=secret,id=DIRECTUS_TOKEN \
  echo NEXT_PUBLIC_DIRECTUS_TOKEN=$(cat /run/secrets/DIRECTUS_TOKEN) >> .env.production
RUN yarn build

# If using npm comment out above and use below instead
# RUN npm run build

# Production image, copy all the files and run next
FROM gcr.io/distroless/nodejs18-debian12:nonroot
WORKDIR /app

ENV NODE_ENV production
# Uncomment the following line in case you want to disable telemetry during runtime.
# ENV NEXT_TELEMETRY_DISABLED 1

# RUN addgroup --system --gid 1001 nodejs
# RUN adduser --system --uid 1001 nextjs

COPY --from=builder /app/public ./public

# Automatically leverage output traces to reduce image size
# https://nextjs.org/docs/advanced-features/output-file-tracing
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

USER nonroot

EXPOSE 3000

ENV PORT 3000

CMD ["server.js"]
