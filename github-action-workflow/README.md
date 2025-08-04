# Composite GitHub Workflows Repository

Welcome to our Composite GitHub Workflows Repository! This repository contains a collection of composite actions that can be used in GitHub Actions workflows across various projects.

## Overview

Composite actions in this repository are designed to simplify and standardize common CI/CD tasks such as linting, testing, building, and deploying applications. By using these composite actions, you can maintain consistency in your workflow processes across multiple projects and reduce duplication.

## Usage

To use a composite action from this repository in your workflow, you'll need to reference it in your `.github/workflows` YAML file. Here's a basic example of how to use a composite action:

```yaml
name: CI/CD on Pull Request

on:
  pull_request:
    types: [ labeled ]

jobs:
  pull_request_api-wrker:
    name: Pull Request deck-test-repo
    if: ${{ github.event.label.name == 'Ready To Test' }}
    uses: pi-financial/github-action-workflow/.github/workflows/nodejs-ci-cd-pr-v1.yml@main
    with:
      things

    secrets:
     secrets
