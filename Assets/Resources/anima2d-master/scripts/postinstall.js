const fs = require('fs');
const path = require('path');
const umm = require('@umm/scripts');

if (umm.libraries.info.development_install) {
  return;
}

// Assets/Modules/umm@anima2d/Editor Default Resources 以下の全てを Assets/ 以下に移動
umm.libraries.synchronize(
  path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Editor Default Resources'),
  path.join(umm.libraries.info.base_path, 'Assets', 'Editor Default Resources'),
  [
    '**/*',
  ],
  {
    overwrite: false,
    remove_source: true,
    remove_empty_source_directory: true,
  }
);
// Assets/Modules/umm@anima2d/Editor Default Resources ディレクトリを削除
if (
    fs.existsSync(path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Editor Default Resources'))
    && !fs.readdirSync(path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Editor Default Resources')).some((item) => !/^\.+$/.test(item))
) {
  fs.rmdirSync(path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Editor Default Resources'));
}

// Assets/Modules/umm@anima2d/Gizmos 以下の全てを Assets/ 以下に移動
umm.libraries.synchronize(
  path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Gizmos'),
  path.join(umm.libraries.info.base_path, 'Assets', 'Gizmos'),
  [
    '**/*',
  ],
  {
    overwrite: false,
    remove_source: true,
    remove_empty_source_directory: true,
  }
);
// Assets/Modules/umm@anima2d/Gizmos ディレクトリを削除
if (
    fs.existsSync(path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Gizmos'))
    && !fs.readdirSync(path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Gizmos')).some((item) => !/^\.+$/.test(item))
) {
  fs.rmdirSync(path.join(umm.libraries.info.base_path, 'Assets', 'Modules', umm.libraries.info.module_name, 'Gizmos'));
}