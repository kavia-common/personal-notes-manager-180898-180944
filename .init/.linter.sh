#!/bin/bash
cd /home/kavia/workspace/code-generation/personal-notes-manager-180898-180944/notes_app_backend
dotnet build --no-restore -v quiet -nologo -consoleloggerparameters:NoSummary /p:TreatWarningsAsErrors=false
LINT_EXIT_CODE=$?
if [ $LINT_EXIT_CODE -ne 0 ]; then
  exit 1
fi

