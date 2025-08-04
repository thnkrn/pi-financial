import { PrismaClient } from '@prisma/client';

const prisma = new PrismaClient();
async function main() {
  await prisma.$transaction([
    prisma.plan.create({
      data: {
        title: '6-month',
        description: '',
        product: 'mt5',
        month: 6,
        price: 1620,
      },
    }),
    prisma.plan.create({
      data: {
        title: '12-month',
        description: '',
        product: 'mt5',
        month: 12,
        price: 3240,
      },
    }),
    prisma.plan.create({
      data: {
        title: '3-month',
        description: '',
        product: 'mt5',
        month: 3,
        price: 810,
      },
    }),
  ]);
}

main()
  .then(async () => {
    await prisma.$disconnect();
  })
  .catch(async (e) => {
    console.error(e);
    await prisma.$disconnect();
    process.exit(1);
  });
