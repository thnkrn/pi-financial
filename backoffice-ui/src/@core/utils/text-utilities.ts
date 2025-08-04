export function transformToUppercase(text: string) {
  return text.toUpperCase()
}

export function isURL(str: string): boolean {
  const urlPattern = /^(http|https|ftp):\/\/[^\s/$.?#].[^\s]*$/i

  return urlPattern.test(str)
}

export function getFileNameFromUrl(url: string) {
  const isUrl = /^(http|https|ftp):\/\/[^\s/$.?#].[^\s]*$/i.test(url)

  if (isUrl) {
    const lastSlashIndex = url.lastIndexOf('/')
    const fileNameWithExtension = url.substring(lastSlashIndex + 1)

    return fileNameWithExtension
  } else {
    return url
  }
}

export function renderCellWithNA(value: string | boolean | null) {
  return value || 'N/A'
}
