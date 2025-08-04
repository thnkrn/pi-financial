import { Side, OC } from 'src/constants/blocktrade/GlobalEnums'

type SideMapProp = {
  [key: string]: {
    side: string
    oc: string
  }
}

export const sideMap: SideMapProp = {
  LONG: {
    side: Side.LONG,
    oc: OC.OPEN,
  },
  SHORT: {
    side: Side.SHORT,
    oc: OC.CLOSE,
  },
  COVERBUY: {
    side: Side.LONG,
    oc: OC.CLOSE,
  },
  SHORTSELL: {
    side: Side.SHORT,
    oc: OC.OPEN,
  },
}
