module.exports = {
  locales: ['en-US', 'th-TH'],
  defaultLocale: 'en-US',
  pages: {
    '*': ['common'],
    '/home': ['homepage'],
    '/central-workspace': ['central_workspace'],
    '/ocr-portal': ['ocr_portal'],
    '/dashboard': ['dashboard'],
    '/application-summary': ['application_summary'],
    '/document-portal': ['document_portal'],
  },
  loadLocaleFrom: async (lang, ns) => {
    const res = await fetch(
      `${process.env.NEXT_PUBLIC_DIRECTUS_PROJECT_URL}/backoffice_ui_contents?access_token=${process.env.NEXT_PUBLIC_DIRECTUS_TOKEN}&fields=key,namespace,translations.value,status&filter[status][_eq]=published&filter[namespace][_in]=${ns}&deep[translations][_filter][languages_code][_eq]=${lang}`
    )
    const response = await res.json()

    // NOTE: clean up the data model that we got from directus and
    // use to iterate over the array of objects and merge each object into the accumulator object.
    // The result is a single object containing all the key-value pairs from the original array.
    const translations = response.data.reduce((acc, item) => ({ ...acc, [item.key]: item.translations[0].value }), {})

    // NOTE: must return as promise for next-translate
    return Promise.resolve(translations)
  },
}
