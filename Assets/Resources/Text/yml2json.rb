require 'yaml'
require 'json'

yml = YAML.load_file('story.yml')

File.open("story.txt", "w") do |io|
  io.write JSON.dump(yml)
end
