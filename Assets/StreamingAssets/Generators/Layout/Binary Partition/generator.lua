require 'utils'

function initialize_layout (settings)
	return new_layout()
end

function iterate (layout, settings)
	local width = math.random(settings.min_map_width, settings.max_map_width)
	local height = math.random(settings.min_map_height, settings.max_map_height)
	local tree = new_node(new_int2(0, 0), new_int2(width, height))
	split_tree(tree, settings)
	build_layout(tree, layout)
	return layout
end

function is_done (layout, settings)
	return table_length(layout.zones) > 0
end

-- helpers

function new_node(min_bound, max_bound)
	local result = { }
	result.min_bound = min_bound
	result.max_bound = max_bound
	return result
end

function split_tree(node, settings)
	local min_width = math.floor((node.max_bound.x - node.min_bound.x) / 2)
	local can_split_horizontal = min_width >= settings.min_zone_width

	local min_height = math.floor((node.max_bound.y - node.min_bound.y) / 2)
	local can_split_vertical = min_height >= settings.min_zone_height

	if can_split_horizontal or can_split_vertical then
		local min_random = 0.5
		local max_random = 0.5

		if can_split_horizontal then
			min_random = 0
		end

		if can_split_vertical then
			max_random = 1
		end

		local random_result = math.random() * (max_random - min_random) + min_random

		if random_result >= 0.5 then
			local min_split_point = node.min_bound.y + settings.min_zone_height
			local max_split_point = node.max_bound.y - settings.min_zone_height
			local split_point = math.random(min_split_point, max_split_point)

			node.child_a = new_node(node.min_bound, new_int2(node.max_bound.x, split_point))
			node.child_b = new_node(new_int2(node.min_bound.x, split_point), node.max_bound)
		else 
			local min_split_point = node.min_bound.x + settings.min_zone_width
			local max_split_point = node.max_bound.x - settings.min_zone_width
			local split_point = math.random(min_split_point, max_split_point)

			node.child_a = new_node(node.min_bound, new_int2(split_point, node.max_bound.y))
			node.child_b = new_node(new_int2(split_point, node.min_bound.y), node.max_bound)
		end

		split_tree(node.child_a, settings)
		split_tree(node.child_b, settings)
	end
end

function build_layout(tree, layout)
	if tree.child_a ~= nil and tree.child_b ~= nil then
		connect_zones(layout, tree)
	else
		local zone = new_zone(tree.min_bound, tree.max_bound)
		tree.zone_id = zone.id
		layout.zones[zone.id] = zone
	end

	layout.initial_zone = get_initial_zone(tree)
	layout.final_zone = get_final_zone(tree)
end

function connect_zones(layout, node)
	local result = { }

	if is_leaf(node) then
		local zone = new_zone(node.min_bound, node.max_bound)
		node.zone_id = zone.id
		layout.zones[zone.id] = zone
		table.insert(result, zone)
	else
		local zone_list_a = connect_zones(layout, node.child_a)
		local zone_list_b = connect_zones(layout, node.child_b)

		local adjacent_pairs = get_adjacent_pairs(zone_list_a, zone_list_b)
		local index = math.random(1, table_length(adjacent_pairs))
		local picked_pair = adjacent_pairs[index]
		add_layout_connection(layout, picked_pair.a, picked_pair.b)

		for i, zone in ipairs(zone_list_a) do
			table.insert(result, zone)
		end

		for i, zone in ipairs(zone_list_b) do
			table.insert(result, zone)
		end
	end

	return result
end

function get_adjacent_pairs(list_a, list_b)
	local result = { }

	for i, value_a in ipairs(list_a) do
		for j, value_b in ipairs(list_b) do
			if are_adjacent(value_a, value_b) and table_length(get_contact_tiles(value_a, value_b)) > 1 then
				local pair = { }
				pair.a = value_a
				pair.b = value_b
				table.insert(result, pair)
			end
		end
	end

	return result
end

function is_leaf(node)
	local result = false

	if (node ~= nil and node.child_a == nil and node.child_b == nil) then
		result = true
	end

	return result
end

function get_initial_zone(node)
	local result = -1

	if is_leaf(node) then
		result = node.zone_id
	else
		result = get_initial_zone(node.child_a)
	end

	return result
end

function get_final_zone(node)
	local result = -1

	if is_leaf(node) then
		result = node.zone_id
	else
		result = get_final_zone(node.child_b)
	end

	return result
end
