from PyInstaller.utils.hooks import copy_metadata

# Include the metadata for streamlit
datas = copy_metadata('streamlit')
