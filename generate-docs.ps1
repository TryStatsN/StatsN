cp -Recurse src/StatsN docfx_project/src/StatsN
docfx build docfx_project/docfx.json
cp -Recurse docfx_project/_site docs/
rm -Recurse -Force docfx_project/src/**
rm -Recurse -Force docfx_project/_site/**