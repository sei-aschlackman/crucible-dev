FROM mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm

ARG TARGETARCH
ARG DEBIAN_FRONTEND=noninteractive

# Switch to root for configuring image
USER root

# Custom Cert Support
COPY ./certs /usr/local/share/ca-certificates/custom/
RUN find /usr/local/share/ca-certificates/custom -type f ! -name '*.crt' -delete \
    && if find /usr/local/share/ca-certificates/custom -type f -name '*.crt' -print -quit | grep -q .; then \
    update-ca-certificates; \
    fi

ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt \
    AWS_CA_BUNDLE=/etc/ssl/certs/ca-certificates.crt \
    REQUESTS_CA_BUNDLE=/etc/ssl/certs/ca-certificates.crt \
    PIP_CERT=/etc/ssl/certs/ca-certificates.crt

# required for those running ARM builds, possibly confined only to Angular 16. Try back later.
RUN apt update
RUN apt install -y python3-dev

# Switch back to vscode user
USER vscode