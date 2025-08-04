import * as diff from 'diff'

const styles = {
  added: {
    color: 'red',
    backgroundColor: '#fec4c0',
  },
  removed: {
    color: 'red',
    backgroundColor: '#fec4c0',
  },
  common: {},
}

export const TextDiff = ({ firstString, secondString }: { firstString: string; secondString: string }) => {
  const changes = diff.diffChars(firstString, secondString, { ignoreCase: true })

  const displayText = changes.map(change => {
    const { value, added, removed } = change
    let charStyle

    if (added) {
      charStyle = styles.added
    } else if (removed) {
      charStyle = styles.removed
    } else {
      charStyle = styles.common
    }

    return (
      <span key={value} style={charStyle}>
        {value}
      </span>
    )
  })

  return <span>{displayText}</span>
}
