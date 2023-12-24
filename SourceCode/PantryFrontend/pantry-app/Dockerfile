FROM node:20-alpine AS build

WORKDIR /app
COPY . .
RUN NODE_ENV=development npm install
RUN npm run build

FROM node:20-alpine AS deploy-node

WORKDIR /app
RUN rm -rf ./*
COPY --from=build /app/package.json .
COPY --from=build /app/build .

ENV NODE_ENV=production
EXPOSE 3000

CMD ["node", "index.js"]
