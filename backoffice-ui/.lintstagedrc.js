module.exports = {
  // Type check TypeScript files
  '**/*.(ts|tsx)': () => 'yarn tsc --noEmit',

  // Lint & Prettify TS and JS files
  '**/!((*.config|*.d|.*)).(ts|tsx|js)': filenames => [
    `yarn run lint ${filenames.join(' ')}`,
    `yarn run prettier --write ${filenames.join(' ')}`,
  ],

  // Prettify only Markdown, JSON, HTML files
  '**/*.(md|json|html)': filenames => `yarn prettier --write ${filenames.join(' ')}`,
}
