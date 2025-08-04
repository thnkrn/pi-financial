import { TasksProcessRequest } from './tasks-process-request.dto';

export class UpdateSubscriptionRequest extends TasksProcessRequest {
  monthEndProcess: boolean;
}
