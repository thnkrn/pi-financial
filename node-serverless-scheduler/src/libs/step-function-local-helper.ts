import {
  States,
  serviceName,
  stepFunctionLocalAccountId,
  stepFunctionLocalRegion,
} from '../../serverless';

interface StateResource {
  state: string;
  Resource: string;
}

// not supported yet https://github.com/aws/aws-cdk/issues/23216#issuecomment-1888349175
export function isSupportedState(
  states: States,
  filteredStates: string[] = []
): boolean {
  for (const stateName in states) {
    const state = states[stateName];
    if (state.Iterator?.States) {
      isSupportedState(state.Iterator.States, filteredStates);
    }

    if (state.ItemProcessor?.States) {
      isSupportedState(state.ItemProcessor.States, filteredStates);
    }

    if (state.ItemReader || state.ResultWriter) {
      filteredStates.push(stateName);
    }
  }
  return filteredStates.length == 0;
}

export function getStateResources(
  states: States,
  stateResources: StateResource[] = []
): StateResource[] {
  for (const stateName in states) {
    const state = states[stateName];
    if (state.Resource) {
      stateResources.push({
        state: stateName,
        Resource: getResource(state.Resource),
      });
    }
    if (state.Iterator?.States) {
      getStateResources(state.Iterator.States, stateResources);
    }

    if (state.ItemProcessor?.States) {
      getStateResources(state.ItemProcessor.States, stateResources);
    }
  }
  return stateResources;
}

function getResource(resource?: string | { 'Fn::GetAtt': string[] }) {
  let result = '';
  if (typeof resource === 'string') {
    result = resource;
  } else {
    const name = (result = resource['Fn::GetAtt'][0]);
    result = `arn:aws:lambda:${stepFunctionLocalRegion}:${stepFunctionLocalAccountId}:function:${serviceName}-dev-${name}`;
  }
  return result;
}
