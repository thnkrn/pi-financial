// ** React Imports
import { useCallback } from 'react'

// ** MUI Imports
import { Grid } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'

// ** Custom Components Imports
import PRICE_INTERVALS from '@/constants/blocktrade/PriceIntervals'
import { Side } from 'src/constants/blocktrade/GlobalEnums'
import { CalculationResult, DataEntry } from 'src/types/blocktrade/calculator/result'
import TopFirstCol from './TopFirstCol'
import TopForthCol from './TopForthCol'
import TopSecondCol from './TopSecondCol'
import TopThirdCol from './TopThirdCol'
import { useSelector } from 'react-redux'

interface FillDataProps {
  onCalculation: React.Dispatch<React.SetStateAction<CalculationResult | null>>
}

const FillData = ({ onCalculation }: FillDataProps) => {
  const calculatorStore = useSelector((state: any) => state.btCalculator)

  const handleCalculate = useCallback(async () => {
    let marketOpenFee = 0

    if (calculatorStore.openPrice) {
      if (calculatorStore.openPrice <= 100) {
        marketOpenFee = calculatorStore.symbol.substring(0, 3) === 'SCB' ? 5.1 : 0.51 // Hardcode for SCB
      } else {
        marketOpenFee = 5.1
      }
    }
    const dividendReturn = 0.9
    const vatFee = 0.07
    const commissionFee = calculatorStore.commFee / 100
    const minimumIntPerShare = 0.005
    const shareAmount =
      (calculatorStore.contractAmount ? calculatorStore.contractAmount : 0) *
      (calculatorStore.futuresProperty.mul ? calculatorStore.futuresProperty.mul : 0)
    const im =
      (calculatorStore.futuresProperty.im ? calculatorStore.futuresProperty.im : 0) *
      (calculatorStore.contractAmount ? calculatorStore.contractAmount : 0)
    const timeToMaturity = Math.round(
      (new Date(calculatorStore.futuresProperty ? calculatorStore.futuresProperty.exp : 0).getTime() -
        new Date(calculatorStore.openDate).getTime()) /
        (1000 * 60 * 60 * 24)
    )
    const futuresPriceOpen = calculatorStore.openPrice
    const commissionFeeOpen =
      ((calculatorStore.openPrice ? calculatorStore.openPrice : 0) * shareAmount * commissionFee +
        marketOpenFee * (calculatorStore.contractAmount ? calculatorStore.contractAmount : 0)) *
      (1 + vatFee) // Com Fee 0.10% + Market Fee * VAT 7%
    const holdingPeriodPre = Math.round(
      (new Date(calculatorStore.closeDate).getTime() - new Date(calculatorStore.openDate).getTime()) /
        (1000 * 60 * 60 * 24)
    )
    const holdingPeriod = holdingPeriodPre < calculatorStore.minDay ? calculatorStore.minDay : holdingPeriodPre
    let interestPerShare =
      ((calculatorStore.openPrice ? calculatorStore.openPrice : 0) *
        (calculatorStore.intRate ? calculatorStore.intRate / 100 : 0) *
        holdingPeriod) /
      365
    if (interestPerShare < minimumIntPerShare) {
      interestPerShare = minimumIntPerShare
    }
    let futuresPriceClose = 0
    let initialPnL = 0
    let PnLAfterInt = 0
    if (calculatorStore.side === Side.LONG) {
      futuresPriceClose =
        Number(calculatorStore.closePrice) - interestPerShare + Number(calculatorStore.xd) * dividendReturn
      futuresPriceClose = Number(futuresPriceClose.toFixed(4))
      initialPnL =
        (calculatorStore.closePrice - calculatorStore.openPrice + calculatorStore.xd * dividendReturn) * shareAmount // Before Com and Int Fee
      PnLAfterInt = (futuresPriceClose - futuresPriceOpen) * shareAmount
    } else if (calculatorStore.side === Side.SHORT) {
      futuresPriceClose = Number(calculatorStore.closePrice) + interestPerShare + Number(calculatorStore.xd)
      futuresPriceClose = Number(futuresPriceClose.toFixed(4))
      initialPnL = (calculatorStore.openPrice - calculatorStore.closePrice - calculatorStore.xd) * shareAmount // Before Com and Int Fee
      PnLAfterInt = (futuresPriceOpen - futuresPriceClose) * shareAmount
    }
    const marketCloseFee = futuresPriceClose <= 100 ? 0.51 : 5.1
    const futuresPriceCloseRounded = Math.round(Number(futuresPriceClose) * 10000) / 10000
    const commissionFeeClose =
      (futuresPriceCloseRounded * shareAmount * commissionFee + marketCloseFee * calculatorStore.contractAmount) *
      (1 + vatFee)
    const totalCommissionFee = commissionFeeOpen + commissionFeeClose

    const totalInterestAmount = initialPnL - PnLAfterInt
    const netPnL = PnLAfterInt - totalCommissionFee

    // Price Interval
    const priceIntervalData = PRICE_INTERVALS.find(
      data =>
        data.lower_price <= (calculatorStore.closePrice ? calculatorStore.closePrice : 0) &&
        data.upper_price > (calculatorStore.closePrice ? calculatorStore.closePrice : 0)
    )
    const priceInterval = priceIntervalData ? priceIntervalData.interval : 0
    const upper: { [key: string]: DataEntry } = {}
    const lower: { [key: string]: DataEntry } = {}
    let futuresPriceCloseUpper = {}
    let futuresPriceCloseLower = {}
    let marketCloseFeeUpper
    let marketCloseFeeLower

    for (let i = 1; i <= 7; i++) {
      if (calculatorStore.side === Side.LONG) {
        futuresPriceCloseUpper = Number(
          (
            Number(calculatorStore.closePrice) +
            priceInterval * (8 - i) -
            interestPerShare +
            Number(calculatorStore.xd) * dividendReturn
          ).toFixed(4)
        )
        futuresPriceCloseLower = Number(
          (
            Number(calculatorStore.closePrice) -
            priceInterval * i -
            interestPerShare +
            Number(calculatorStore.xd) * dividendReturn
          ).toFixed(4)
        )
      } else if (calculatorStore.side === Side.SHORT) {
        futuresPriceCloseUpper = Number(
          (
            Number(calculatorStore.closePrice) +
            priceInterval * (8 - i) +
            interestPerShare +
            Number(calculatorStore.xd)
          ).toFixed(4)
        )
        futuresPriceCloseLower = Number(
          (
            Number(calculatorStore.closePrice) -
            priceInterval * i +
            interestPerShare +
            Number(calculatorStore.xd) * dividendReturn
          ).toFixed(4)
        )
      }

      // Market Open Fee
      if ((futuresPriceCloseUpper as number) <= 100) {
        marketCloseFeeUpper = 0.51
      } else {
        marketCloseFeeUpper = 5.1
      }
      if ((futuresPriceCloseLower as number) <= 100) {
        marketCloseFeeLower = 0.51
      } else {
        marketCloseFeeLower = 5.1
      }
      const futuresPriceCloseUpperRounded = Math.round(Number(futuresPriceCloseUpper) * 10000) / 10000
      const comFeeCloseUpper =
        (futuresPriceCloseUpperRounded * shareAmount * commissionFee +
          marketCloseFeeUpper * calculatorStore.contractAmount) *
        (1 + vatFee)
      const totalComFeeUpper = commissionFeeOpen + comFeeCloseUpper
      const futuresPriceCloseLowerRounded = Math.round(Number(futuresPriceCloseLower) * 10000) / 10000
      const comFeeCloseLower =
        (futuresPriceCloseLowerRounded * shareAmount * commissionFee +
          marketCloseFeeLower * calculatorStore.contractAmount) *
        (1 + vatFee)
      const totalComFeeLower = commissionFeeOpen + comFeeCloseLower
      const upperData: DataEntry = {
        id: i,
        closeStockPrice: Number(calculatorStore.closePrice) + priceInterval * (8 - i),
        openSsfPrice: calculatorStore.openPrice,
        closeSsfPrice: futuresPriceCloseUpperRounded,
        pnl:
          (calculatorStore.side === Side.LONG
            ? futuresPriceCloseUpperRounded - calculatorStore.openPrice
            : calculatorStore.openPrice - futuresPriceCloseUpperRounded) *
            shareAmount -
          totalComFeeUpper,
      }
      upper[i.toString()] = upperData
      const lowerData: DataEntry = {
        id: i,
        closeStockPrice: Number(calculatorStore.closePrice) - priceInterval * i,
        openSsfPrice: calculatorStore.openPrice,
        closeSsfPrice: futuresPriceCloseLowerRounded,
        pnl:
          (calculatorStore.side === Side.LONG
            ? futuresPriceCloseLowerRounded - calculatorStore.openPrice
            : calculatorStore.openPrice - futuresPriceCloseLowerRounded) *
            shareAmount -
          totalComFeeLower,
      }
      lower[i.toString()] = lowerData
    }

    const response = {
      lastDayTrading: calculatorStore.futuresProperty?.exp || '',
      openDate: calculatorStore.openDate,
      holdingPeriod: holdingPeriodPre,
      symbol: calculatorStore.futuresProperty?.value || '',
      priceInterval,
      totalCommissionFee,
      initialPnL,
      PnLAfterInt,
      netPnL,
      openPosition: {
        futuresPriceOpen,
        im,
        timeToMaturity,
        commissionFeeOpen,
        cashUsage: im + commissionFeeOpen,
      },
      closePosition: {
        futuresPriceClose,
        interestPerShare,
        holdingPeriod: holdingPeriodPre,
        totalInterestAmount,
        commissionFeeClose,
      },
      projUpper: {
        data: upper,
      },
      projMiddle: {
        data: {
          1: {
            id: 1,
            closeStockPrice: Number(calculatorStore.closePrice),
            openSsfPrice: calculatorStore.openPrice,
            closeSsfPrice: futuresPriceClose,
            pnl:
              (calculatorStore.side === Side.LONG
                ? futuresPriceClose - calculatorStore.openPrice
                : calculatorStore.openPrice - futuresPriceClose) *
                shareAmount -
              totalCommissionFee,
          },
        },
      },
      projLower: {
        data: lower,
      },
    }

    onCalculation(response)
  }, [
    calculatorStore.symbol,
    calculatorStore.closeDate,
    calculatorStore.commFee,
    onCalculation,
    calculatorStore.openDate,
    calculatorStore.closePrice,
    calculatorStore.contractAmount,
    calculatorStore.intRate,
    calculatorStore.minDay,
    calculatorStore.openPrice,
    calculatorStore.side,
    calculatorStore.futuresProperty,
    calculatorStore.xd,
  ])

  return (
    <div>
      <Card>
        <CardContent>
          <Grid container spacing={2} rowGap={1}>
            {/* columns 1 */}
            <TopFirstCol />
            {/* columns 2 */}
            <TopSecondCol />
            {/* columns 3 */}
            <TopThirdCol />
            {/* columns 4 */}
            <TopForthCol onCalculate={handleCalculate} />
          </Grid>
        </CardContent>
      </Card>
    </div>
  )
}

export default FillData
