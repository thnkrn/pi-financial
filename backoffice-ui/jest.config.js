const nextJest = require('next/jest')

const createJestConfig = nextJest({
  // Provide the path to your Next.js app to load next.config.js and .env files in your test environment
  dir: './',
})

// Add any custom config to be passed to Jest
const customJestConfig = {
  setupFilesAfterEnv: ['<rootDir>/jest.setup.js'],
  testEnvironment: 'jest-environment-jsdom',
  moduleNameMapper: {
    '^@/@core/*': '<rootDir>/src/@core/$1',
    '^@/pages/(.*)$': '<rootDir>/src/pages/$1',
    '^@/layouts/(.*)$': '<rootDir>/src/layouts/$1',
    '^@/utils/(.*)$': '<rootDir>/src/utils/$1',
    '^@/lib/(.*)$': '<rootDir>/src/lib/$1',
    '^@/hooks/(.*)$': '<rootDir>/src/hooks/$1',
    '^@/components/(.*)$': '<rootDir>/src/@core/components/$1',
    '^@/views/(.*)$': '<rootDir>/src/views/$1',
  },
  modulePathIgnorePatterns: ['<rootDir>/.next'],
  testPathIgnorePatterns: ['<rootDir>/node_modules/', '<rootDir>/.next/'],
  globalSetup: '<rootDir>/global-setup.js',
}

// createJestConfig is exported this way to ensure that next/jest can load the Next.js config which is async
module.exports = createJestConfig(customJestConfig)
