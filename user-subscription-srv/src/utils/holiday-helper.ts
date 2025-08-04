import { getItem, scan } from '@libs/dynamodb-client';
import { getDateStr, isWeekend } from '@utils/date-time-utils';
import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { isDevelopment } from './env-helper';

dayjs.extend(utc);
dayjs.extend(timezone);

const bangkokTimeZone = 'Asia/Bangkok';

export type Holiday = {
  holidayDate: string;
};

export class HolidayHelper {
  /**
   * Check if date is holiday
   * @param dateStr
   */
  async isHoliday(dateStr?: string): Promise<boolean> {
    const todayDateStr =
      dateStr ?? getDateStr({ format: 'YYYY-MM-DD', tz: 'Asia/Bangkok' });
    const holiday = (await getItem(process.env.DYNAMODB_HOLIDAY_TABLE_NAME, {
      holidayDate: todayDateStr,
    })) as Holiday;
    return !!holiday;
  }

  /**
   * Get holiday list
   */
  async getHolidayList(): Promise<Holiday[]> {
    return (await scan(process.env.DYNAMODB_HOLIDAY_TABLE_NAME))
      .Items as Holiday[];
  }

  async getHolidaySet(): Promise<Set<string>> {
    let holidaySet: Set<string>;
    if (!isDevelopment()) {
      const holidayList = await this.getHolidayList();
      holidaySet = new Set(holidayList.map((holiday) => holiday.holidayDate));
    } else {
      holidaySet = new Set([
        '2023-12-05',
        '2023-12-11',
        '2024-01-01',
        '2024-01-02',
      ]);
    }
    return holidaySet;
  }

  isNonWorkingDay(holidaySet: Set<string>, startDate: dayjs.Dayjs) {
    return (
      holidaySet.has(startDate.format('YYYY-MM-DD')) ||
      isWeekend({
        dateStr: startDate.format('YYYY-MM-DD'),
        tz: bangkokTimeZone,
      })
    );
  }

  /**
   * Find first day of week that is not holiday (First day of week is Monday)
   * @param dateStr
   */
  async findNextEffectiveDate(dateStr: string): Promise<string> {
    const holidaySet = await this.getHolidaySet();

    let startDate = dayjs(dateStr)
      .tz(bangkokTimeZone)
      .add(1, 'day')
      .add(-1, 'minute');

    while (this.isNonWorkingDay(holidaySet, startDate)) {
      startDate = dayjs(startDate).tz(bangkokTimeZone).add(1, 'day');
    }

    return startDate.format('YYYYMMDD');
  }

  async findStartOfMonthEffectiveDate(dateStr: string): Promise<string> {
    const holidaySet = await this.getHolidaySet();
    let startDate = dayjs(dateStr).tz(bangkokTimeZone).add(1, 'day');

    while (this.isNonWorkingDay(holidaySet, startDate)) {
      startDate = dayjs(startDate).tz(bangkokTimeZone).add(1, 'day');
    }

    return startDate.format('YYYYMMDD');
  }

  async findEndOfMonthEffectiveDate(dateStr: string): Promise<string> {
    const holidaySet = await this.getHolidaySet();
    let startDate = dayjs(dateStr).tz(bangkokTimeZone);

    while (this.isNonWorkingDay(holidaySet, startDate)) {
      startDate = dayjs(startDate).tz(bangkokTimeZone).subtract(1, 'day');
    }

    return startDate.format('YYYYMMDD');
  }
}
