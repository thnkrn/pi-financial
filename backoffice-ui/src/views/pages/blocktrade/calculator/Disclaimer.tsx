import { Typography } from '@mui/material'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import CardHeader from '@mui/material/CardHeader'

const Disclaimer = () => {
  return (
    <div>
      <Card sx={{ marginRight: { xs: 0, lg: 2 }, marginTop: 3, height: '100%' }}>
        <CardHeader
          sx={{ paddingTop: 4, paddingBottom: 0 }}
          title={<Typography sx={{ color: 'primary.main', fontWeight: 700 }}>นิยาม</Typography>}
        />
        <CardContent sx={{ fontSize: '14px', marginBottom: 0 }}>
          XD คือ เงินปันผลที่เกิดขึ้นจริง หมายถึง เงินปันผลที่หลักทรัพย์อ้างอิงได้ประกาศวันที่ XD
          ในเวปไซต์ตลาดหลักทรัพย์แห่งประเทศไทยและอยู่ในช่วงเวลาที่ลูกค้ายังมีสถานะเปิดกับทางบริษัทหลักทรัพย์ พาย จำกัด
          (มหาชน)
        </CardContent>
        <CardHeader
          sx={{ paddingTop: 0, paddingBottom: 0, marginTop: -4 }}
          title={<Typography sx={{ color: 'primary.main', fontWeight: 700 }}>Disclaimer</Typography>}
        />
        <CardContent sx={{ fontSize: '14px' }}>
          โปรแกรมนี้จัดทำโดย บริษัทหลักทรัพย์ พาย จำกัด (มหาชน) (“บล.พาย”) มีวัตถุประสงค์เพื่ออำนวยความสะดวกให้แก่
          ผู้แนะนำการลงทุน ให้สามารถคำนวณข้อมูลเบื้องต้นเกี่ยวกับธุรกกรม Block Trade เพื่อการลงทุนใน Single Stock
          Futures กับ บล.พาย ได้สะดวกรวดเร็วขึ้นเท่านั้น มิได้มุ่งหมายเพื่อให้เสนอซื้อ เสนอขาย ซื้อหรือขาย เชิญชวน จูงใจ
          ส่งเสริม หรือเป็นการยืนยันการทำรายการซื้อขายเกี่ยวกับหลักทรัพย์ หรือผลิตภัณฑ์ทางการเงินที่เกี่ยวข้องแต่อย่างใด
          ผลลัพธ์ที่ได้จากโปรแกรมดังกล่าวเป็นเพียงการนำข้อมูลพื้นฐานมาคำนวณทางคณิตศาสตร์
          ผลลัพธ์จึงอาจเปลี่ยนแปลงไปตามข้อมูลที่ได้กำหนดขึ้น หรือมีความคลาดเคลื่อนได้ โดย บล.พาย
          ไม่รับประกันหรือรับรองในความถูกต้องของผลลัพธ์ที่ได้จากโปรแกรมดังกล่าว
        </CardContent>
      </Card>
    </div>
  )
}

export default Disclaimer
