export const DateTimeToDate = (datetime: string) => {
  return new Date(datetime).toISOString().split('T')[0]
}

export const FormatDateTime = (dateTimeString: string) => {
  const dateTime = new Date(dateTimeString)

  return dateTime.toLocaleString('en-GB', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  })
}
