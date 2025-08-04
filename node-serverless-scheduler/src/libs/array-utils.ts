/**
 * Filters out null or empty strings from the input array.
 *
 * @param {string[]} array - the input array of strings
 * @return {string[]} the filtered array of strings
 */
export const filteredArray = (array: string[]) =>
  array.filter((s): s is string => s !== null && s.trim() !== '');
