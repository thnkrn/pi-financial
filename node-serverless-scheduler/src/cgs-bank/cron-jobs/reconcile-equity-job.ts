import reconcileJobs from '@cgs-bank/cron-jobs/reconcile-job';

(async () => {
  try {
    await reconcileJobs.start('equity');
    process.exit(0);
  } catch (error) {
    console.log(error);
    process.exit(1);
  }
})();
