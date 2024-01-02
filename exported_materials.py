import json
import bpy
import os

# List to store material data
all_materials_data = []

# Iterate over all materials in the project
for material in bpy.data.materials:
    # Create a dictionary with material properties
    material_data = {
        "name": material.name,
        "diffuse_color": list(material.diffuse_color),
        "specular_color": list(material.specular_color),
        "specular_intensity": material.specular_intensity,
    }

    # Try to access emission if nodes are present
    if material.use_nodes:
        emission_node = material.node_tree.nodes.get("Emission")
        if emission_node:
            material_data["emit"] = list(emission_node.inputs["Color"].default_value)[:3]

    # Check if transparency is enabled
    if material.blend_method == 'HASHED' or material.blend_method == 'CLIP':
        material_data["use_transparency"] = True

    # Add the material data to the list
    all_materials_data.append(material_data)

# Encode the list of material data as JSON
data = json.dumps(all_materials_data, indent=1, ensure_ascii=True)

# Set the output path and file name
save_path = 'D:/Venus_test'
file_name = os.path.join(save_path, "export_all_materials.json")

# Write the JSON file
with open(file_name, 'w') as outfile:
    outfile.write(data + '\n')

print(f"Material data exported to: {file_name}")
