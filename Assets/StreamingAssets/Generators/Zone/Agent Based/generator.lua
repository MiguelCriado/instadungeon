require 'utils'

directions = { }
directions[0] = new_int2(0, 1)
directions[1] = new_int2(1, 0)
directions[2] = new_int2(0, -1)
directions[3] = new_int2(-1, 0)

zones_contact_tiles = { }

function pre_connect_zones(map, settings)
	zones_contact_tiles = { }

	for key, value in pairs(map.layout.connections) do
		if (value ~= nil) then
			local zone_a = map.layout.zones[key]

			for i, zone_id in ipairs(value) do
				local zone_b = map.layout.zones[zone_id]
				local tiles = get_contact_tiles(zone_a, zone_b)

				if table_length(tiles) > 0 then
					if zones_contact_tiles[zone_a.id] == nil then
						zones_contact_tiles[zone_a.id] = { }
					end

					zones_contact_tiles[zone_a.id][zone_b.id] = tiles
				end
			end
		end
	end

	return map
end

function generate(map, settings)
	for zone_id, zone in pairs(map.layout.zones) do
		generate_zone(zone, zones_contact_tiles[zone.id], settings)
	end

	return map
end

function post_connect_zones(map, settings)
	local starting_tiles = get_starting_tiles(map)

	for zone_id, zone in pairs(map.layout.zones) do
		for other_zone_id, tile in pairs(starting_tiles[zone.id]) do
			connect_zone(zone, zones_contact_tiles[zone.id], tile, settings)
		end
	end

	return map
end

function get_spawn_point(map, settings)
	local initial_zone = map.layout.zones[map.layout.initial_zone]
	return find_stairs_tile(initial_zone)
end

function get_exit_point(map, settings)
	local final_zone = map.layout.zones[map.layout.final_zone]
	return find_stairs_tile(final_zone)
end

-- helpers

function find_stairs_tile(zone)
	local visited_tiles = { }

	local initial_tile = new_int2(0, 0)
	initial_tile.x = math.random(zone.min_bound.x + 1, zone.max_bound.x - 2)
	initial_tile.y = math.random(zone.min_bound.y + 1, zone.max_bound.y - 2)

	while (zone.tiles[initial_tile.x][initial_tile.y] == false) do
		initial_tile.x = math.random(zone.min_bound.x + 1, zone.max_bound.x - 2)
		initial_tile.y = math.random(zone.min_bound.y + 1, zone.max_bound.y - 2)
	end

	return flood_find_stairs_tile(zone, initial_tile, zones_contact_tiles[zone.id], visited_tiles)
end

function flood_find_stairs_tile(zone, tile, contact_tiles, visited_tiles)
	local result = nil

	if is_valid_tile(zone, contact_tiles, tile) == true
		and zone.tiles[tile.x][tile.y] == true
		and (visited_tiles[tile.x] == nil or visited_tiles[tile.x][tile.y] == nil) then

		local up = get_next_tile(tile, directions[0])
		local right = get_next_tile(tile, directions[1])
		local down = get_next_tile(tile, directions[2])
		local left = get_next_tile(tile, directions[3])

		if is_valid_tile(zone, contact_tiles, up) == true
			and zone.tiles[up.x][up.y] == true
			and is_valid_tile(zone, contact_tiles, right) == true
			and zone.tiles[right.x][right.y] == true
			and is_valid_tile(zone, contact_tiles, down) == true
			and zone.tiles[down.x][down.y] == true
			and is_valid_tile(zone, contact_tiles, left) == true
			and zone.tiles[left.x][left.y] == true then

			result = tile
		else
			add_to_tiles_record(visited_tiles, tile)
			local adjacent_tiles = { }
			adjacent_tiles[1] = up
			adjacent_tiles[2] = right
			adjacent_tiles[3] = down
			adjacent_tiles[4] = left
			local i = 1

			while (result == nil and i < 5) do
				result = flood_find_stairs_tile(zone, adjacent_tiles[i], contact_tiles, visited_tiles)
				i = i + 1
			end
		end
	end

	return result
end

function generate_zone(zone, contact_tiles, settings)
	local direction = get_random_direction()
	local change_direction_chance = settings.direction_change_increment
	local place_room_chance = settings.room_placement_increment

	local total_tiles = (zone.max_bound.x - zone.min_bound.x) * (zone.max_bound.y - zone.min_bound.y)
	local floor_tiles = 0

	local x_initial = math.random(zone.min_bound.x + 1, zone.max_bound.x - 1)
	local y_initial = math.random(zone.min_bound.y + 1, zone.max_bound.y - 1)
	local current_tile = new_int2(x_initial, y_initial)

	while floor_tiles / total_tiles < settings.stop_threshold do
		floor_tiles = floor_tiles + add_tile(zone, contact_tiles, current_tile)

		if math.random() <= place_room_chance then
			floor_tiles = floor_tiles + table_length(place_room(zone, contact_tiles, current_tile, settings))
			place_room_chance = 0
		else
			place_room_chance = math.min(1.0, place_room_chance + settings.room_placement_increment)
		end

		if math.random() <= change_direction_chance then
			direction = get_random_direction()
			change_direction_chance = 0
		else
			change_direction_chance = math.min(1.0, change_direction_chance + settings.direction_change_increment)
		end

		local next_tile = get_next_tile(current_tile, directions[direction])

		while is_valid_tile(zone, contact_tiles, next_tile) == false do
			direction = get_next_direction(direction)
			next_tile = get_next_tile(current_tile, directions[direction])
		end

		current_tile = next_tile
	end
end

function connect_zone(zone, contact_tiles, starting_tile, settings)
	local direction = get_random_direction()
	local change_direction_chance = settings.direction_change_increment
	local place_room_chance = settings.room_placement_increment
	local new_tiles = { }
	local connected = false
	local current_tile = starting_tile

	while connected == false do
		if check_connection(zone, new_tiles, current_tile) then
			connected = true
		else
			if add_tile(zone, contact_tiles, current_tile) == 1 then
				add_to_tiles_record(new_tiles, current_tile)
			end
			
			if math.random() <= place_room_chance then
				local room_new_tiles, room_origin, room_extents = place_room(zone, contact_tiles, current_tile, settings)

				for i, tile in ipairs(room_new_tiles) do
					add_to_tiles_record(new_tiles, tile)
				end

				local tile = new_int2(0, 0)

				for x = room_origin.x, room_extents.x do 
					tile.x = x

					for y = room_origin.y, room_extents.y do
						tile.y = y
						connected = connected or check_connection(zone, new_tiles, tile)
					end
				end

				place_room_chance = 0
			else
				place_room_chance = math.min(1.0, place_room_chance + settings.room_placement_increment)
			end
	
			if math.random() <= change_direction_chance then
				direction = get_random_direction()
				change_direction_chance = 0
			else
				change_direction_chance = math.min(1.0, change_direction_chance + settings.direction_change_increment)
			end
	
			local next_tile = get_next_tile(current_tile, directions[direction])
	
			while is_valid_tile(zone, contact_tiles, next_tile) == false do
				direction = get_next_direction(direction)
				next_tile = get_next_tile(current_tile, directions[direction])
			end
	
			current_tile = next_tile
		end	
	end
end

function check_connection(zone, new_tiles, tile)
	return zone.tiles[tile.x] ~= nil
		and zone.tiles[tile.x][tile.y] ~= nil
		and zone.tiles[tile.x][tile.y] == true
		and is_in_new_tiles(new_tiles, tile) == false
end

function add_to_tiles_record(tiles_record, tile)
	if tiles_record[tile.x] == nil then
		tiles_record[tile.x] = { }
	end

	tiles_record[tile.x][tile.y] = true
end

function is_in_new_tiles(new_tiles, tile)
	return new_tiles[tile.x] ~= nil and new_tiles[tile.x][tile.y] ~= nil
end

function place_room(zone, contact_tiles, current_tile, settings)
	local room_width = math.random(settings.min_room_width, settings.max_room_width)
	local room_height = math.random(settings.min_room_height, settings.max_room_height)
	local room_origin = new_int2(0, 0)
	room_origin.x = current_tile.x - math.floor(room_width / 2)
	room_origin.y = current_tile.y - math.floor(room_height / 2)
	local room_extents = new_int2(room_origin.x + room_width, room_origin.y + room_height)
	return add_floor_tiles(zone, contact_tiles, room_origin, room_extents), room_origin, room_extents
end

function add_floor_tiles(zone, contact_tiles, min_bound, max_bound)
	local tiles_added = { }
	local tile = new_int2(0, 0)

	for x = min_bound.x, max_bound.x do
		tile.x = x
		for y = min_bound.y, max_bound.y do
			tile.y = y

			if add_tile(zone, contact_tiles, tile) == 1 then
				table.insert(tiles_added, new_int2(x, y))
			end
		end
	end

	return tiles_added
end

function add_tile(zone, contact_tiles, tile)
	local tile_is_new = 0
	local tiles = zone.tiles

	if is_valid_tile(zone, contact_tiles, tile) == true then
		if tiles[tile.x] == nil then
			tiles[tile.x] = { }
		end

		if tiles[tile.x][tile.y] == nil or tiles[tile.x][tile.y] == false then
			tile_is_new = 1
		end

		tiles[tile.x][tile.y] = true
	end

	return tile_is_new
end

function get_random_direction()
	return math.random(0, 3)
end

function get_next_direction(direction)
	local result = 0

	if direction < 3 then
		result = direction + 1
	end

	return result
end

function get_next_tile(current, direction)
	return new_int2(current.x + direction.x, current.y + direction.y)
end

function is_valid_tile(zone, contact_tiles, tile)
	local result = true

	if ((tile.x <= zone.min_bound.x
		or tile.x >= zone.max_bound.x - 1
		or tile.y <= zone.min_bound.y
		or tile.y >= zone.max_bound.y - 1)
		and contains_tile(contact_tiles, tile) == false) then
		result = false
	end

	return result
end

function contains_tile(contact_tiles, tile)
	result = false

	for i, zone_tiles in pairs(contact_tiles) do
		for j, contact_tile in pairs(zone_tiles) do
			if (tile.x == contact_tile.x and tile.y == contact_tile.y) then
				return true
			end
		end
	end

	return result
end

function get_starting_tiles(map)
	local result = { }

	for zone_a_id, zone_a in pairs(map.layout.zones) do
		for zone_b_index, zone_b_id in pairs(map.layout.connections[zone_a_id]) do
			if result[zone_a_id] == nil then
				result[zone_a_id] = { }
			end

			if result[zone_a_id][zone_b_id] == nil then
				local existing_a_tiles = find_existing_floor_tiles(zone_a, zones_contact_tiles[zone_a_id][zone_b_id])
				local zone_b = map.layout.zones[zone_b_id]
				local existing_b_tiles = find_existing_floor_tiles(zone_b, zones_contact_tiles[zone_b_id][zone_a_id])
				local starting_tile_a
				local starting_tile_b

				if table_length(existing_a_tiles) > 0 then
					starting_tile_a = existing_a_tiles[math.random(1, table_length(existing_a_tiles))]
					starting_tile_b = get_contact_point(zone_b, starting_tile_a)
				elseif table_length(existing_b_tiles) > 0 then
					starting_tile_b = existing_b_tiles[math.random(1, table_length(existing_b_tiles))]
					starting_tile_a = get_contact_point(zone_a, starting_tile_b)
				else 
					local a_contact_tiles = zones_contact_tiles[zone_a_id][zone_b_id]
					starting_tile_a = a_contact_tiles[math.random(1, table_length(a_contact_tiles))]
					starting_tile_b = get_contact_point(zone_b, starting_tile_a)
				end

				result[zone_a_id][zone_b_id] = starting_tile_a
				
				if result[zone_b_id] == nil then
					result[zone_b_id] = { }
				end

				result[zone_b_id][zone_a_id] = starting_tile_b
			end
		end
	end

	return result
end

function get_contact_point(zone, tile)
	local result = nil

	local x = zone.min_bound.x
	local y = zone.min_bound.y
	local width = zone.max_bound.x - zone.min_bound.x
	local height = zone.max_bound.y - zone.min_bound.y

	if tile.y >= y and tile.y < y + height then
		if tile.x == x - 1 then
			result = new_int2(x, tile.y)
		elseif tile.x == x + width then
			result = new_int2(tile.x - 1, tile.y)
		end
	end

	if result == nil and tile.x >= x and tile.x < x + width then
		if tile.y == y - 1 then
			result = new_int2(tile.x, y)
		elseif tile.y == y + height then
			result = new_int2(tile.x, tile.y - 1)
		end
	end

	return result
end

function find_existing_floor_tiles(zone, tiles_to_check)
	local result = { }

	for i, tile in ipairs(tiles_to_check) do
		if zone.tiles[tile.x][tile.y] == true then
			table.insert(result, tile)
		end
	end

	return result
end
