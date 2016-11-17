import * as coreJs from "core-js/shim";
import {AppRegistry} from 'react-native';
import {runnable} from './out/Nightwatch';

AppRegistry.registerRunnable('nightwatch', runnable);