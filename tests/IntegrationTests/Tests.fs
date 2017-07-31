module IntegrationTests.Tests

open IntegrationTests.ExpectoHelper
open canopy.mobile
open Expecto

configuration.elementTimeout <- configuration.elementTimeout * 3.

let tests = 
    testList "All tests" [
        SoundCheck.tests
    ]
