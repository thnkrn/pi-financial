// ** Next Import
import { GetStaticPaths, GetStaticProps, GetStaticPropsContext, InferGetStaticPropsType } from 'next/types'

// ** Components Imports
import BlocktradePage from 'src/views/pages/blocktrade/BlocktradePage'

const BlocktradeView = ({ tab }: InferGetStaticPropsType<typeof getStaticProps>) => {
  return <BlocktradePage tab={tab} />
}

export const getStaticPaths: GetStaticPaths = () => {
  return {
    paths: [
      { params: { tab: 'dashboard' } },
      { params: { tab: 'allocation' } },
      { params: { tab: 'portfolio' } },
      { params: { tab: 'activity-logs' } },
      { params: { tab: 'calculator' } },
      { params: { tab: 'monitor' } },
      { params: { tab: 'margin-report' } },
    ],
    fallback: false,
  }
}

export const getStaticProps: GetStaticProps = async ({ params }: GetStaticPropsContext) => {
  return {
    props: {
      tab: params?.tab,
    },
  }
}

export default BlocktradeView
