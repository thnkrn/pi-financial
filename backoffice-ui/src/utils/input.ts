import React from 'react'

export const inputAlphaNumeric = (e: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
  const value = e.target.value ? e.target.value.replace(/[^0-9a-zA-Z\s]+/gi, '') : ''

  if (e.target.value !== value) {
    e.target.value = value
  }

  return value
}

export const inputAlphabet = (e: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
  const value = e.target.value ? e.target.value.replace(/[^a-zA-Z\s]+/gi, '') : ''

  if (e.target.value !== value) {
    e.target.value = value
  }

  return value
}
