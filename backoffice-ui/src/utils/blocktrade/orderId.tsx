export const extractNumbers = (str: string) => {
  const regex = /(\d+)/
  const matches = regex.exec(str)

  return matches ? parseInt(matches[0], 10) : 0
}

export const orderIdComparator = (v1: string, v2: string) => {
  const num1 = extractNumbers(v1)
  const num2 = extractNumbers(v2)

  return num1 - num2
}
